namespace design_patterns.Messaging.MessageArgs; 

public class NewNodeConnectedArgs {
    public string Socket { get; private set; }
    
    public NewNodeConnectedArgs(string socket)
    {
        this.Socket = socket;
    }
}