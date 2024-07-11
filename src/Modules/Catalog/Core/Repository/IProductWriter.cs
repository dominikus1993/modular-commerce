using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Core.Types;

namespace Modular.Ecommerce.Catalog.Core.Repository;


public interface IProductsWriter
{
    Task<Result<Unit>> AddProduct(Product product, CancellationToken cancellationToken = default);
    
    Task<Result<Unit>> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default);

    Task<Result<Unit>> RemoveAllProducts(CancellationToken cancellationToken = default);
}