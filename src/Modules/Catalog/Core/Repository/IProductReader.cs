using Modular.Ecommerce.Catalog.Core.Model;

namespace Modular.Ecommerce.Catalog.Core.Repository;

internal interface IProductReader
{
    Task<Product?> GetById(ProductId id, CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<Product> GetByIds(IEnumerable<ProductId> ids, CancellationToken cancellationToken = default);
}