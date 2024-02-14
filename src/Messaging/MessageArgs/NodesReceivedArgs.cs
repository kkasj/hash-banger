namespace design_patterns.Messaging.MessageArgs;

public class NodesReceivedArgs {
    public List<string> Sockets { get; private set; }
    
    public NodesReceivedArgs(List<string> sockets)
    {
        this.Sockets = sockets;
    }
}