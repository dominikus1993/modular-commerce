using System.Runtime.CompilerServices;
using System.Text.Json;
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
    private readonly IChannel _bus;
    private readonly IMessageSerializer<T> _messageSerializer;
    private readonly RabbitMqPublisherConfiguration<T> _publisherConfiguration;
    
    public RabbitMqMessagePublisher(IChannel bus, IMessageSerializer<T> messageSerializer, RabbitMqPublisherConfiguration<T> publisherConfiguration)
    {
        _bus = bus;
        _messageSerializer = messageSerializer;
        _publisherConfiguration = publisherConfiguration;
    }

    public async ValueTask<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        var props = PrepareProperties(message, _publisherConfiguration);
        var messageBody = _messageSerializer.Serialize(message);
        await _bus.BasicPublishAsync(_publisherConfiguration.ExchangeName, _publisherConfiguration.RouteKey, false, props, messageBody, cancellationToken);
        return Result.UnitResult;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BasicProperties PrepareProperties(T message, RabbitMqPublisherConfiguration<T> config)
    {
        ArgumentNullException.ThrowIfNull(message);
        var properties = new BasicProperties { MessageId = message.MessageId, Type = MessageType.FullName, ContentType = "application/json", Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()), Expiration = config.Ttl?.TotalMilliseconds.ToString() };
        return properties;
    }
}