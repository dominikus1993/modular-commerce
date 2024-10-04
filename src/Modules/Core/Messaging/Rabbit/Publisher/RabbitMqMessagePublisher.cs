using System.Runtime.CompilerServices;
using System.Text.Json;
using EasyNetQ;
using Modular.Ecommerce.Core.Messaging.Abstraction;
using Modular.Ecommerce.Core.Types;
using RabbitMQ.Client;
using IMessage = Modular.Ecommerce.Core.Messaging.Abstraction.IMessage;

namespace Modular.Ecommerce.Core.Messaging.Rabbit.Publisher;

public interface IMessageSerializer<T> where T : IMessage
{
    public ReadOnlyMemory<byte> Serialize(T message);
}

public sealed class SystemTextMessageSerializer<T> : IMessageSerializer<T> where T : IMessage
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SystemTextMessageSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public ReadOnlyMemory<byte> Serialize(T message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
    } 
}

public sealed class RabbitMqPublisherConfiguration<T> where T : IMessage
{
    public string ExchangeName { get; }
    public string RouteKey { get; }
    
    public TimeSpan? Ttl { get; }
}

internal sealed class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    private static readonly Type MessageType = typeof(T);
    private readonly IAdvancedBus _bus;
    private readonly IMessageSerializer<T> _messageSerializer;
    private readonly RabbitMqPublisherConfiguration<T> _publisherConfiguration;
    
    public RabbitMqMessagePublisher(IAdvancedBus bus, IMessageSerializer<T> messageSerializer, RabbitMqPublisherConfiguration<T> publisherConfiguration)
    {
        _bus = bus;
        _messageSerializer = messageSerializer;
        _publisherConfiguration = publisherConfiguration;
    }

    public async ValueTask<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        var props = PrepareProperties(message, _publisherConfiguration);
        var msg = Message<T>.Create(message, props);
        await _bus.PublishAsync(_publisherConfiguration.ExchangeName, _publisherConfiguration.RouteKey, false, msg, cancellationToken: cancellationToken);
        return Result.UnitResult;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MessageProperties PrepareProperties(T message, RabbitMqPublisherConfiguration<T> config)
    {
        ArgumentNullException.ThrowIfNull(message);
        var messageProps = new MessageProperties { MessageId = message.MessageId, Type = MessageType.FullName, ContentType = "application/json", Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Expiration = config.Ttl };
        return messageProps;
    }
}

internal sealed class Message<T> : IMessage<T>
{
    private static readonly Type CachedType = typeof(T);
    public Type MessageType { get; }
    public T Body { get; }

    public object? GetBody() { return Body; }
    public MessageProperties Properties { get; }

    private Message(T body, MessageProperties properties, Type messageType)
    {
        Body = body;
        Properties = properties;
        MessageType = messageType;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Message<T> Create(T body, MessageProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(body);
        return new Message<T>(body, properties, CachedType);
    }
}