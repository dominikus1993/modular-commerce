using Catalog.Infrastructure.Extensions;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Modular.Ecommerce.Catalog.Infrastructure.Model;
using Modular.Ecommerce.Core.Types;
using OpenSearch.Client;
using OpenSearch.Net;
using Result = Modular.Ecommerce.Core.Types.Result;

namespace Modular.Ecommerce.Catalog.Infrastructure.Repositories;

public sealed class OpenSearchProductsWriter : IProductsWriter
{
    private readonly IOpenSearchClient _openSearchClient;

    public OpenSearchProductsWriter(IOpenSearchClient openSearchClient)
    {
        _openSearchClient = openSearchClient;
    }

    public async Task<Result<Unit>> AddProduct(Product product, CancellationToken cancellationToken = default)
    {
        var openSearchProduct = new OpenSearchProduct(product);
        var response = await _openSearchClient.IndexAsync(openSearchProduct, descriptor => descriptor.Id(openSearchProduct.ProductId).Refresh(Refresh.True)
                .Index(OpenSearchProductIndex.Name), cancellationToken);
        
        if (!response.IsValid)
        {
            return Result.Failure<Unit>(new InvalidOperationException("can't add product to opensearch", response.OriginalException));
        }
        var refreshRes = await _openSearchClient.Indices.RefreshAsync(OpenSearchProductIndex.Name, ct: cancellationToken);
        
        if (!refreshRes.IsValid)
        {
            return Result.Failure<Unit>(new InvalidOperationException("can't refresh product index in opensearch", response.OriginalException));
        }

        return Result.UnitResult;
    }

    public async Task<Result<Unit>> AddProducts(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        var openSearchProduct = products.Select(product => new OpenSearchProduct(product));
        var response = await _openSearchClient.IndexManyAsync(openSearchProduct, OpenSearchProductIndex.Name, cancellationToken: cancellationToken);
        if (!response.IsValid)
        {
            return Result.Failure<Unit>(new InvalidOperationException("can't add products to opensearch", response.OriginalException));
        }
        
        var refreshRes = await _openSearchClient.Indices.RefreshAsync(OpenSearchProductIndex.Name, ct: cancellationToken);
        
        if (!refreshRes.IsValid)
        {
            return Result.Failure<Unit>(new InvalidOperationException("can't refresh products index in opensearch", response.OriginalException));
        }

        return Result.UnitResult;
    }

    public async Task<Result<Unit>> RemoveAllProducts(CancellationToken cancellationToken = default)
    {
        var result = 
            await _openSearchClient.DeleteByQueryAsync<OpenSearchProduct>(q => q.Index(OpenSearchProductIndex.SearchIndex).Query(d => d.MatchAll()), cancellationToken);
        if (!result.IsValid)
        {
            return Result.Failure<Unit>(new InvalidOperationException("remove documents error"));
        }
        return Result.UnitResult;
    }
}