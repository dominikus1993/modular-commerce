namespace Modular.Ecommerce.Core.Messaging.Abstraction;

public interface IMessage
{
    string MessageId { get; init; }
    long Timestamp { get; init; }
}

public abstract class Message : IMessage
{
    public string MessageId { get; init; }
    public long Timestamp { get; init; }
    
    protected Message(string messageId, long timestamp)
    {
        MessageId = messageId;
        Timestamp = timestamp;
    }
}