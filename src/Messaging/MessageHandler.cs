using System.Text.Json;
using design_patterns.Messaging.Exceptions;
using design_patterns.Messaging.MessageArgs;

namespace design_patterns.Messaging; 

public class MessageHandler : RangeUpdater, ISubscriber {
    private readonly LocalPeer _localPeer;
    private Problem _problem;
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
        // take updates whose source is Sync
        var syncUpdates = updates.Where(update => update.UpdateSource == UpdateSource.Local);
        // delete them from the original list
        if (syncUpdates.Count() > 0)
            _problem.IteratorProxy.Updates = updates.Except(syncUpdates).ToList();
        
        // send updates to other peers
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
    private void HandleMessage(string newMessage) {
        Message msg = Deserialize(newMessage);
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
                ProblemArgs newProblemArgs = JsonSerializer.Deserialize<ProblemArgs>(newMessage);
                _problem.GotNewProblem(newProblemArgs.ProblemHash, newProblemArgs.EncryptionType);
                //TODO: handle iterator
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