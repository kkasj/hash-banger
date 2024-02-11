using System.Text.Json;
using design_patterns.Messaging.Exceptions;
using design_patterns.Messaging.MessageArgs;

namespace design_patterns.Messaging; 

public class MessageHandler {
    private readonly LocalPeer _localPeer;

    public MessageHandler(LocalPeer localPeer) {
        _localPeer = localPeer;
        _localPeer.MessageReceived += HandleMessage;
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
                throw new NotImplementedException();
            case MessageType.CancelProblem:
                throw new NotImplementedException();
            case MessageType.ProblemSolved:
                throw new NotImplementedException();
            case MessageType.RangeStarted:
                throw new NotImplementedException();
            case MessageType.RangeCompleted:
                throw new NotImplementedException();
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