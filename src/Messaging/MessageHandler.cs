using Lidgren.Network;
using System.Text.Json;
using design_patterns.Messaging.Exceptions;
using design_patterns.Messaging.MessageArgs;
using System.Collections.Concurrent;

namespace design_patterns.Messaging; 

public class MessageHandler : RangeUpdater, ISubscriber {
    private readonly LocalPeer _localPeer;
    private Problem _problem;
    private ProblemArgs? _lastProblemArgs = null;
    private bool _receivedResult = false;
    private bool _sentResult = false;
    public MessageHandler(LocalPeer localPeer, Problem problem) {
        _problem = problem;
        problem.Subscribe(this);
        _localPeer = localPeer;
        _localPeer.MessageReceived += HandleMessage;
        _localPeer.OnNewNodeConnected += (connection) => {
            // if the problem is not yet set, it means this is the new peer
            if (_problem.Args is null) {
                return;
            }
            SendProblemToNode(connection);
            Console.WriteLine("New node connected - sent problem.");
        };
        Source = UpdateSource.Sync;
    }

    public void Poke() {
        if (_problem.Args is not null && _problem.Args.IsDone) {
            if (!_receivedResult && !_sentResult) {
                _sentResult = true;
                var endMessage = new Message(MessageType.ProblemSolved, _problem.Args.Result);
                _localPeer.BroadcastMessage(endMessage);
            }
            return;
        }

        // the problem changed - send the updated problem to other peers
        // if (_lastProblemArgs != _problem.Args) {
        //     BroadcastNewProblem();
        //     _lastProblemArgs = _problem.Args;
        //     return;
        // }
        
        var rangeUpdates = _problem.IteratorProxy.Updates;

        // take updates whose source is Local
        var localUpdates = rangeUpdates.Where(update => update.UpdateSource == UpdateSource.Local);
        
        // send range updates to other peers
        foreach (var update in localUpdates) {
            switch (update.UpdateType) {
                case UpdateType.RangeDone:
                    var messageRangeDone = new Message(MessageType.RangeCompleted, JsonSerializer.Serialize(update.Range));
                    _localPeer.BroadcastMessage(messageRangeDone);
                    break;
                case UpdateType.RangeReservation:
                    var messageRangeReservation = new Message(MessageType.RangeReserved, JsonSerializer.Serialize((ReservedRange)update.Range));
                    _localPeer.BroadcastMessage(messageRangeReservation);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        // delete them from the original concurrent bag
        if (localUpdates.Count() > 0){
            _problem.IteratorProxy.Updates = new ConcurrentBag<RangeUpdate>(rangeUpdates.Except(localUpdates));
        }
    }
    private void HandleMessage(string newMessage, NetConnection sender) {
        Message msg = Deserialize(newMessage);
        // Console.WriteLine("Received message: " + msg.Type);
        switch (msg.Type) {
            case MessageType.Hello:
                Console.WriteLine("Hello received");
                break;
            case MessageType.NewNodeConnected:
                _localPeer.NodeConnected(JsonSerializer.Deserialize<NewNodeConnectedArgs>(msg.Content));
                break;
            case MessageType.NodeDisconnected:
                _localPeer.NodeDisconnected(JsonSerializer.Deserialize<NodeDisconnectedArgs>(msg.Content));
                break;
            case MessageType.NodesReceived:
                _localPeer.NodesReceived(JsonSerializer.Deserialize<NodesReceivedArgs>(msg.Content));
                break;
            case MessageType.NewProblem:
                NewProblemArgs newProblemArgs = JsonSerializer.Deserialize<NewProblemArgs>(msg.Content);
                _problem.GotNewProblem(newProblemArgs);

                Console.WriteLine("New problem received - ", newProblemArgs.ProblemHash);

                break;
            case MessageType.CancelProblem:
                throw new NotImplementedException();
            case MessageType.ProblemSolved:
                _receivedResult = true;
                _problem.ProblemSolved(msg.Content);
                break;
            case MessageType.RangeReserved:
                ReservedRange reservedRange = JsonSerializer.Deserialize<ReservedRange>(msg.Content);
                _problem.ReserveTask(this, reservedRange);
                break;
            case MessageType.RangeCompleted:
                Range doneRange = JsonSerializer.Deserialize<Range>(msg.Content);
                _problem.TaskDone(this, doneRange);
                break;
            case MessageType.ProblemDiscovery:
                NewProblemArgs newProblemArgs2 = JsonSerializer.Deserialize<NewProblemArgs>(msg.Content);

                // ProblemDiscovery is directed to new peers only
                if (_problem.Args is null) {
                    _problem.GotNewProblem(newProblemArgs2);
                }

                break;
        }
    }

    public void BroadcastNewProblem() {
        var newProblemArgs = new NewProblemArgs(_problem.Args.ProblemHash, _problem.Args.EncryptionType, null);
        var newProblemMessage = new Message(MessageType.NewProblem, JsonSerializer.Serialize(newProblemArgs));
        _localPeer.BroadcastMessage(newProblemMessage);
    }

    public void SendProblemToNode(NetConnection connection) {
        var newProblemArgs = new NewProblemArgs(_problem.Args.ProblemHash, _problem.Args.EncryptionType, _problem.IteratorProxy.Iterator);
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