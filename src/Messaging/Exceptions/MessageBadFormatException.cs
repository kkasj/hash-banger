namespace design_patterns.Messaging.Exceptions;

public class MessageBadFormatException : FormatException {
    public MessageBadFormatException() { }

    public MessageBadFormatException(string message)
        : base(message) { }

    public MessageBadFormatException(string message, Exception inner)
        : base(message, inner) { }
}