using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.Repository;
using Modular.Ecommerce.Catalog.Core.UseCase.Requests;

namespace Modular.Ecommerce.Catalog.Core.UseCase;

public sealed class GetProductByIdUseCase
{
    private readonly IProductReader _productReader;

    public GetProductByIdUseCase(IProductReader productReader)
    {
        _productReader = productReader;
    }

    public async Task<ProductDetailsDto?> Execute(GetProductById request, CancellationToken cancellationToken = default)
    {
        var product = await _productReader.GetById(request.ProductId, cancellationToken);
        return product is not null ? new ProductDetailsDto(product) : null;
    }
}