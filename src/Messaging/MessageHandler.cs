using Lidgren.Network;
using System.Text.Json;
using System.Collections.Concurrent;
using design_patterns.Messaging.Exceptions;
using design_patterns.Messaging.MessageArgs;
using design_patterns.Encryption;
using design_patterns.ProblemManagement;
using design_patterns.TaskRanges;

namespace design_patterns.Messaging;

public class MessageHandler : Subscriber {
    private readonly LocalPeer _localPeer;

    public MessageHandler(LocalPeer localPeer, Problem problem) {
        this.problem = problem;
        visitor = new MessageHandlerProblemUpdateVisitor(this);
        problem.Subscribe(this);
        _localPeer = localPeer;
        _localPeer.MessageReceived += HandleMessage;
        _localPeer.OnNewNodeConnected += (connection) => {
            // if the problem is not yet set, it means this is the new peer
            if (this.problem.Args is null) {
                return;
            }
            // if the problem is done, we don't need to send it
            if (this.problem.Args.IsDone) {
                return;
            }
            SendProblemToNewNode(connection);
            Console.WriteLine("New node connected - sent problem.");
        };
    }

    private void HandleMessage(string newMessage) {
        Message msg = Deserialize(newMessage);
        // Console.WriteLine("Received message: " + msg.Type);
        switch (msg.Type) {
            case MessageType.Hello:
                Console.WriteLine("Hello received");
                break;
            case MessageType.NewNodeConnected:
                _localPeer.NodeConnected(JsonSerializer.Deserialize<NewNodeConnectedArgs>(msg.Content) ?? throw new MessageBadFormatException("NewNodeConnected message contains null newNodeConnectedArgs"));
                break;
            case MessageType.NodeDisconnected:
                _localPeer.NodeDisconnected(JsonSerializer.Deserialize<NodeDisconnectedArgs>(msg.Content) ?? throw new MessageBadFormatException("NodeDisconnected message contains null nodeDisconnectedArgs"));
                break;
            case MessageType.NodesReceived:
                _localPeer.NodesReceived(JsonSerializer.Deserialize<NodesReceivedArgs>(msg.Content) ?? throw new MessageBadFormatException("NodesReceived message contains null nodesReceivedArgs"));
                break;
            case MessageType.NewProblem:
                NewProblemArgs newProblemArgs = JsonSerializer.Deserialize<NewProblemArgs>(msg.Content) ?? throw new MessageBadFormatException("NewProblem message contains null newProblemArgs");
                problem.GotNewProblem(this, newProblemArgs);

                Console.WriteLine("New problem received - ", newProblemArgs.ProblemHash);
                break;
            case MessageType.CancelProblem:
                throw new NotImplementedException();
            case MessageType.ProblemSolved:
                problem.ProblemSolved(this, msg.Content);
                break;
            case MessageType.RangeReserved:
                TaskRange range = JsonSerializer.Deserialize<TaskRange>(msg.Content) ?? throw new MessageBadFormatException("RangeReserved message contains null range");
                problem.ReserveRange(this, range);
                break;
            case MessageType.RangeCompleted:
                TaskRange doneRange = JsonSerializer.Deserialize<TaskRange>(msg.Content) ?? throw new MessageBadFormatException("RangeCompleted message contains null range");
                problem.RangeDone(this, doneRange);
                break;
            case MessageType.ProblemDiscovery:
                NewProblemArgs newProblemArgs2 = JsonSerializer.Deserialize<NewProblemArgs>(msg.Content) ?? throw new MessageBadFormatException("ProblemDiscovery message contains null newProblemArgs");

                // ProblemDiscovery is directed to new peers only
                if (problem.Args is null) {
                    problem.GotNewProblem(this, newProblemArgs2);
                }
                break;
        }
    }

    public void BroadcastResult(string result) {
        var endMessage = new Message(MessageType.ProblemSolved, result);
        _localPeer.BroadcastMessage(endMessage);
    }

    public void BroadcastRangeDone(TaskRange range) {
        var rangeDoneMessage = new Message(MessageType.RangeCompleted, JsonSerializer.Serialize(range));
        _localPeer.BroadcastMessage(rangeDoneMessage);
    }

    public void BroadcastRangeReservation(TaskRange range) {
        var rangeReservationMessage = new Message(MessageType.RangeReserved, JsonSerializer.Serialize(range));
        _localPeer.BroadcastMessage(rangeReservationMessage);
    }

    public void BroadcastNewProblem() {
        var newProblemArgs = new NewProblemArgs(problem.Args.ProblemHash, problem.Args.EncryptionType, null);
        var newProblemMessage = new Message(MessageType.NewProblem, JsonSerializer.Serialize(newProblemArgs));
        _localPeer.BroadcastMessage(newProblemMessage);
    }

    public void SendProblemToNewNode(NetConnection connection) {
        var newProblemArgs = new NewProblemArgs(problem.Args.ProblemHash, problem.Args.EncryptionType, problem.Iterator.TaskRangeCollection);
        var newProblemMessage = new Message(MessageType.ProblemDiscovery, JsonSerializer.Serialize(newProblemArgs));
        _localPeer.SendMessage(connection, newProblemMessage);
    }


    public static Message Deserialize(string message) {
        var messageObject = JsonSerializer.Deserialize<Message>(message);
        if (messageObject is null) {
            throw new MessageBadFormatException($"[{message}] is not a valid message.");
        }
        return messageObject;
    }
    
    public static string Serialize(Message message) {
        return JsonSerializer.Serialize(message);
    }
}