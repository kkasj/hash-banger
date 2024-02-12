using Lidgren.Network;
using System.Text.Json;
using design_patterns.Messaging.Exceptions;
using design_patterns.Messaging.MessageArgs;

namespace design_patterns.Messaging; 

public class MessageHandler : RangeUpdater, ISubscriber {
    private readonly LocalPeer _localPeer;
    private Problem _problem;
    private ProblemArgs _lastProblemArgs;
    public UpdateSource Source = UpdateSource.Sync;
    public MessageHandler(LocalPeer localPeer, Problem problem) {
        _problem = problem;
        problem.Subscribe(this);
        _localPeer = localPeer;
        _localPeer.MessageReceived += HandleMessage;
    }

    public void Poke() {
        if (_problem.Args.IsDone) {
            var endMessage = new Message(MessageType.ProblemSolved, _problem.Args.Result);
            _localPeer.BroadcastMessage(endMessage);
            return;
        }
        
        var updates = _problem.IteratorProxy.Updates;
        // take updates whose source is Local
        var syncUpdates = updates.Where(update => update.UpdateSource == UpdateSource.Local);
        // delete them from the original list
        if (syncUpdates.Count() > 0)
            _problem.IteratorProxy.Updates = updates.Except(syncUpdates).ToList();

        // the problem changed - send the updated problem to other peers
        if (_lastProblemArgs != _problem.Args) {
            var newProblemArgs = new NewProblemArgs(_problem.Args.ProblemHash, _problem.Args.EncryptionType, null);
            var newProblemMessage = new Message(MessageType.NewProblem, JsonSerializer.Serialize(newProblemArgs));
            _localPeer.BroadcastMessage(newProblemMessage);
            _lastProblemArgs = _problem.Args;
            return;
        }
        
        // send range updates to other peers
        foreach (var update in syncUpdates) {
            switch (update.UpdateType) {
                case UpdateType.RangeDone:
                    var messageRangeDone = new Message(MessageType.RangeCompleted, JsonSerializer.Serialize(update.Range));
                    _localPeer.BroadcastMessage(messageRangeDone);
                    break;
                case UpdateType.RangeReservation:
                    var messageRangeReservation = new Message(MessageType.RangeStarted, JsonSerializer.Serialize(update.Range));
                    _localPeer.BroadcastMessage(messageRangeReservation);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
    private void HandleMessage(string newMessage, NetConnection sender) {
        Message msg = Deserialize(newMessage);
        switch (msg.Type) {
            case MessageType.Hello:
                Console.WriteLine("Hello received");
                break;
            case MessageType.NewNodeConnected:
                _localPeer.NodeConnected(JsonSerializer.Deserialize<NewNodeConnectedArgs>(msg.Content));
                // send the new peer the current problem
                var newProblemArgs1 = new NewProblemArgs(_problem.Args.ProblemHash, _problem.Args.EncryptionType, _problem.IteratorProxy.Iterator);
                var newProblemMessage = new Message(MessageType.NewProblem, JsonSerializer.Serialize(newProblemArgs1));
                _localPeer.SendMessage(sender, newProblemMessage);
            
                break;
            case MessageType.NodeDisconnected:
                _localPeer.NodeDisconnected(JsonSerializer.Deserialize<NodeDisconnectedArgs>(msg.Content));
                break;
            case MessageType.NodesReceived:
                _localPeer.NodesReceived(JsonSerializer.Deserialize<NodesReceivedArgs>(msg.Content));
                break;
            case MessageType.NewProblem:
                NewProblemArgs newProblemArgs = JsonSerializer.Deserialize<NewProblemArgs>(newMessage);
                _problem.GotNewProblem(newProblemArgs);
                break;
            case MessageType.CancelProblem:
                throw new NotImplementedException();
            case MessageType.ProblemSolved:
                _problem.ProblemSolved(newMessage);
                break;
            case MessageType.RangeStarted:
                Range reservedRange = JsonSerializer.Deserialize<Range>(newMessage);
                _problem.ReserveTask(this, reservedRange);
                break;
            case MessageType.RangeCompleted:
                Range doneRange = JsonSerializer.Deserialize<Range>(newMessage);
                _problem.TaskDone(this, doneRange);
                break;
        }
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