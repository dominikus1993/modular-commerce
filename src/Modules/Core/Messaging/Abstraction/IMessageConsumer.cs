using Modular.Ecommerce.Core.Types;

namespace Modular.Ecommerce.Core.Messaging.Abstraction;

public interface IMessageConsumer<T> where T : IMessage
{
    Task<Result<Unit>> ConsumeAsync(T message, IMessageContext context, CancellationToken cancellationToken = default);
}