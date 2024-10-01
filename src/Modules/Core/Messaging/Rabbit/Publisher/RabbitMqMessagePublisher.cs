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
}

internal sealed class RabbitMqMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    private readonly IAdvancedBus _bus;
    private readonly IMessageSerializer<T> _messageSerializer;
    private RabbitMqPublisherConfiguration<T> _publisherConfiguration;
    
    public RabbitMqMessagePublisher(IAdvancedBus bus, IMessageSerializer<T> messageSerializer, RabbitMqPublisherConfiguration<T> publisherConfiguration)
    {
        _bus = bus;
        _messageSerializer = messageSerializer;
        _publisherConfiguration = publisherConfiguration;
    }

    public async ValueTask<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        await _bus.PublishAsync(_publisherConfiguration.ExchangeName, _publisherConfiguration.RouteKey, false, new Message<T>(message), cancellationToken: cancellationToken);
        return Result.UnitResult;
    }
}