using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.UseCase.Requests;

namespace Modular.Ecommerce.Catalog.Core.UseCase;

public sealed class GetProductByIdUseCase
{
    public Task<ProductDetailsDto?> Execute(GetProductById request, CancellationToken cancellationToken = default)
    {
        // Implementation
        
        return Task.FromResult<ProductDetailsDto?>(null);
    }
}