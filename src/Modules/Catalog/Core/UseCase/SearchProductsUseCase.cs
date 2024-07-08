using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Modular.Ecommerce.Catalog.Core.UseCase.Requests;

namespace Modular.Ecommerce.Catalog.Core.UseCase;

public sealed class SearchProductsUseCase
{
    public Task<PagedResult<ProductDto>> SearchProducts(SearchProducts request, CancellationToken cancellationToken = default)
    {
        // Implementation
        
        return Task.FromResult(PagedResult<ProductDto>.Empty);
    }
}