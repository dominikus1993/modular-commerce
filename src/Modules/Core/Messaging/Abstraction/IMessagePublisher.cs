using Modular.Ecommerce.Core.Types;

namespace Modular.Ecommerce.Core.Messaging.Abstraction;

public interface IMessagePublisher<in T> where T : IMessage
{
    ValueTask<Result<Unit>> PublishAsync(T message, CancellationToken cancellationToken = default);
}