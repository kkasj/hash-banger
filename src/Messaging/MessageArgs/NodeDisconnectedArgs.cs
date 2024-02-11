namespace design_patterns.Messaging.MessageArgs; 

public class NodeDisconnectedArgs {
    public string Socket { get; private set; }
    
    public NodeDisconnectedArgs(string socket)
    {
        this.Socket = socket;
    }
}