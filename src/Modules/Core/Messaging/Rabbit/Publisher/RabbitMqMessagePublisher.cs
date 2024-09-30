using System.Text.Json;
using Modular.Ecommerce.Core.Messaging.Abstraction;
using Modular.Ecommerce.Core.Types;
using RabbitMQ.Client;

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
}

internal sealed class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    private readonly IChannel _rabbitMqChannel;
    private readonly IMessageSerializer<T> _messageSerializer;
    private RabbitMqPublisherConfiguration<T> _publisherConfiguration;
    
    public RabbitMqMessagePublisher(IChannel rabbitMqChannel, IMessageSerializer<T> messageSerializer, RabbitMqPublisherConfiguration<T> publisherConfiguration)
    {
        _rabbitMqChannel = rabbitMqChannel;
        _messageSerializer = messageSerializer;
        _publisherConfiguration = publisherConfiguration;
    }

    public async ValueTask<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        var msgBody = _messageSerializer.Serialize(message);
        await _rabbitMqChannel.BasicPublishAsync(_publisherConfiguration.ExchangeName, _publisherConfiguration.RouteKey,
            false, new BasicProperties(), msgBody, cancellationToken);

        return Result.UnitResult;
    }
}