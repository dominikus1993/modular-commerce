using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Modular.Ecommerce.Catalog.Core.UseCase.Requests;

namespace Modular.Ecommerce.Catalog.Core.UseCase;

public sealed class SearchProductsUseCase
{
    private readonly IProductFilter _productFilter;

    internal SearchProductsUseCase(IProductFilter productFilter)
    {
        _productFilter = productFilter;
    }

    public async Task<PagedResult<ProductDto>> SearchProducts(SearchProducts request, CancellationToken cancellationToken = default)
    {
        var res = await _productFilter.FilterProducts(
            new Filter()
            {
                PageSize = request.PageSize,
                Page = request.Page,
                Query = request.Query,
                PriceFrom = request.PriceFrom,
                PriceTo = request.PriceTo
            }, cancellationToken);

        if (res.IsEmpty)
        {
            return PagedResult<ProductDto>.Empty;
        }

        var products = res.Data.Select(p => new ProductDto(p));
        return new PagedResult<ProductDto>(products, res.Metadata, res.Count, res.Total);
    }
}