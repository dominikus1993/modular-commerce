using AutoFixture.Xunit2;
using Catalog.Tests.Fixtures;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Shouldly;

namespace Catalog.Tests.Infrastructure.Repositories;

public sealed class OpenSearchProductsWriterTests : IClassFixture<OpenSearchFixture>, IAsyncLifetime
{
    private readonly OpenSearchFixture _openSearchFixture;
    private readonly IProductReader _productReader;
    private readonly IProductsWriter _productsWriter;
    
    public OpenSearchProductsWriterTests(OpenSearchFixture openSearchFixture)
    {
        _openSearchFixture = openSearchFixture;
        _productsWriter = openSearchFixture.ProductsWriter;
        _productReader = openSearchFixture.ProductReader;
    }

    [Theory]
    [InlineAutoData]
    public async Task WriteProductTest(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.IsSuccess.ShouldBeTrue();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductAndRead(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.IsSuccess.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(product);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductsAndRead(Product product)
    {
        // Act
        var subject = await _productsWriter.AddProducts(new []{product});
        
        subject.IsSuccess.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(product);
    }
    
    [Theory]
    [InlineAutoData]
    public async Task WriteProductTwoTimesTest(Product product, ProductName newProductName)
    {
        // Act
        var subject = await _productsWriter.AddProduct(product);
        
        subject.IsSuccess.ShouldBeTrue();
    
        var newProduct = product with { ProductName = newProductName };
        subject = await _productsWriter.AddProduct(newProduct);

        subject.IsSuccess.ShouldBeTrue();
        
        var productFromDb = await _productReader.GetById(product.Id);
        
        productFromDb.ShouldBeEquivalentTo(newProduct);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _openSearchFixture.Clean();
    }
}