namespace design_patterns.Messaging;

public class Message {
    public MessageType Type { get; private set; }
    public string Content { get; private set; }
    
    public Message(MessageType type, string content)
    {
        this.Type = type;
        this.Content = content;
    }
}