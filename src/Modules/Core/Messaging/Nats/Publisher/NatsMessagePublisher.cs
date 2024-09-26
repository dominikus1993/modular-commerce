using Modular.Ecommerce.Core.Messaging.Abstraction;
using Modular.Ecommerce.Core.Messaging.Nats.Publisher.Configuration;
using Modular.Ecommerce.Core.Types;
using NATS.Client.Core;

namespace Modular.Ecommerce.Core.Messaging.Nats.Publisher;

internal sealed class NatsMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    private readonly NatsConnection _connection;
    private readonly NatsPublisherConfiguration _configuration;
    
    public NatsMessagePublisher(NatsConnection connection, NatsPublisherConfiguration configuration)
    {
        _connection = connection;
        _configuration = configuration;
    }

    public async Task<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.PublishAsync(_configuration.Subject, data: message, cancellationToken: cancellationToken);
            return Result.UnitResult;
        }
        catch (Exception e)
        {
            return Result.Failure<Unit>(e);
        }
    }
}