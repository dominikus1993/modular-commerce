using AutoFixture.Xunit2;

using Catalog.Tests.Fixtures;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Shouldly;

namespace Catalog.Tests.Infrastructure.Repositories;

public sealed class OpenSearchProductFilterTests(OpenSearchFixture fixture) : IAsyncLifetime, IClassFixture<OpenSearchFixture>
{
    private readonly IProductFilter _productFilter = fixture.ProductFilter;
    private readonly IProductsWriter _productsWriter = fixture.ProductsWriter;

    [Theory]
    [InlineAutoData]
    public async Task FilterProductsWithoutAnyPredicates(Product[] products)
    {
        // Arrange
        var res = await _productsWriter.AddProducts(products);
        Assert.True(res.IsSuccess);
        
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { PageSize = products.Length, Page = 1 });

        subject.ShouldNotBeNull();
        subject.IsEmpty.ShouldBeFalse();
        subject.Metadata.ShouldNotBeNull();
        subject.Metadata.TagFiltersMetaData.Filters.ShouldNotBeEmpty();
        subject.Metadata.Prices.ShouldNotBeNull();
        subject.Metadata.Prices.MaxPrice.ShouldBeGreaterThan(0);
        subject.Metadata.Prices.MinPrice.ShouldBeGreaterThan(0);
        subject.Metadata.Prices.MaxPrice.ShouldBeGreaterThan(subject.Metadata.Prices.MinPrice);
        subject.Data.Count().ShouldBe(products.Length);
        subject.Count.ShouldBe((uint)products.Length);
        subject.Total.ShouldBe((uint)products.Length);
        
    }
    
    [Theory]
    [InlineAutoData]
    public async Task ReadFilterProductsWhenNoExistsShouldReturnNull(string query)
    {
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Query = query });

        subject.ShouldNotBeNull();
        subject.IsEmpty.ShouldBeTrue();
    }


    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea" });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.Data.ShouldNotBeEmpty();
    
        await Verify(subject);
    }
    
    [Theory]
    [AutoData]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnProductsWithNameOrDescriptionContainsNiveaTag(string tag, string tag2)
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), new Tags(new []{ new Tag(tag), new Tag(tag2)}), []);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), new Tags(new[] { new Tag(tag2) }), []);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.Data.ShouldNotBeEmpty();
    
        subject.Count.ShouldBe(1u);
        subject.Data.ShouldContain(p => p.Id == product1.Id);
    }
    
    [Theory]
    [AutoData]
    public async Task FilterProductWhenNiveaProductNotExistsShouldReturnEmptyCollectionNiveaTag(Tags tags, string tag2)
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), tags, []);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act

        var subject = await _productFilter.FilterProducts(new Filter() { Tag = tag2 });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.IsEmpty.ShouldBeTrue();
        subject.Data.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task FilterProductWhenProductsInPriceConditionExistsShouldReturnProductsWithPriceCondition()
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(5m), new Price(1m)), new AvailableQuantity(10), Tags.Empty, []);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(11m)), new AvailableQuantity(10), Tags.Empty, []);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(20m), new Price(20m)), new AvailableQuantity(10), Tags.Empty, []);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { PriceFrom = 2m, PriceTo = 12m });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.Data.ShouldNotBeEmpty();
        subject.Total.ShouldBe(1u);
        subject.Count.ShouldBe(1u);
        
        await Verify(subject);
    }
    
    [Fact]
    public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeyword()
    {
        // Arrange 
        var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
        await _productsWriter.AddProducts(new[] { product1, product2, product3 });
        // Act
        
        var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1 });
        
        // Assert
        subject.ShouldNotBeNull();
        subject.Data.ShouldNotBeEmpty();
        subject.Count.ShouldBe(1u);
        
        await Verify(subject);
    }
    
     [Fact]
     public async Task FilterProductWhenNiveaProductExistsShouldReturnOneProductWithNameOrDescriptionContainsNiveaKeywordOnTheSecondPage()
     {
         // Arrange 
         var product1 = new Product(ProductId.New(), new ProductName("not xDDD"), new ProductDescription("nivea"),
             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
         var product2 = new Product(ProductId.New(), new ProductName("Nivea xDDD"), new ProductDescription("xDDD"),
             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
         var product3 = new Product(ProductId.New(), new ProductName("xDDD"), new ProductDescription("xDDD"),
             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), Tags.Empty, []);
         await _productsWriter.AddProducts(new[] { product1, product2, product3 });
         // Act

         var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 1, Page = 2 });
         
         // Assert
         subject.ShouldNotBeNull();
         subject.Data.ShouldNotBeEmpty();
         subject.Count.ShouldBe(1u);
         
         await Verify(subject);
    }

     [Fact]
     public async Task TestFilters()
     {
         // Arrange 
         var product1 = new Product(ProductId.New(), new ProductName("nivea"), new ProductDescription("nivea"),
             new ProductPrice(new Price(10m), new Price(1m)), new AvailableQuantity(10), [new Tag("body")], []);
         var product2 = new Product(ProductId.New(), new ProductName("Nivea"), new ProductDescription("xDDD"),
             new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), [new Tag("hair")], []);
         var product3 = new Product(ProductId.New(), new ProductName("Nivea Men"), new ProductDescription("xDDD"),
             new ProductPrice(new Price(10m), new Price(9m)), new AvailableQuantity(10), [new Tag("hair")], []);
         await _productsWriter.AddProducts([ product1, product2, product3 ]);
         // Act

         var subject = await _productFilter.FilterProducts(new Filter() { Query = "nivea", PageSize = 3, Page = 1 });
         
         // Assert
         subject.ShouldNotBeNull();
         subject.Data.ShouldNotBeEmpty();
         subject.Count.ShouldBe(3u);
         
         await Verify(subject);
     }
     
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return fixture.Clean();
    }
}