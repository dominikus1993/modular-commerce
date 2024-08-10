using Catalog.Infrastructure.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modular.Ecommerce.Catalog.Api.Responses;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.Repository;
using Modular.Ecommerce.Catalog.Core.UseCase;
using Modular.Ecommerce.Catalog.Infrastructure.OpenSearch;
using Modular.Ecommerce.Catalog.Infrastructure.Repositories;
using OpenSearch.Client;

namespace Modular.Ecommerce.Catalog;

public static class Setup
{
    public static WebApplicationBuilder AddCatalog(this WebApplicationBuilder builder)
    {
        var openSearchConfig =
            builder.Configuration.GetSection("Catalog:OpenSearch").Get<OpenSearchConnectionConfiguration>() ??
            throw new InvalidOperationException("OpenSearch configuration is missing");
        var client = OpenSearchInstaller.Setup(openSearchConfig);
        builder.Services.AddSingleton<IOpenSearchClient>(client);
        builder.Services.AddTransient<IProductsWriter, OpenSearchProductsWriter>();
        builder.Services.AddTransient<IProductReader, OpenSearchProductReader>();
        builder.Services.AddTransient<IProductFilter, OpenSearchProductFilter>();
        builder.Services.AddTransient<SearchProductsUseCase>();
        builder.Services.AddTransient<GetProductByIdUseCase>();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddScoped<IValidator<SearchProductsRequest>, SearchProductsRequestValidator>();
        return builder;
    }

    public static async Task InitializeCatalog(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<IOpenSearchClient>();
        await OpenSearchInstaller.CreateIndexIfNotExists(client);
        await app.InitData();
    }


    private static async Task InitData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var writer = scope.ServiceProvider.GetRequiredService<IProductsWriter>();
        var products = GenerateProducts();
        await writer.AddProducts(products);
    }

    private static IEnumerable<Product> GenerateProducts()
    {
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce218")), new ProductName("nivea"), new ProductDescription("nivea cream"),
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), [new Tag("cream"), new Tag("face")], []);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce219")), new ProductName("dove"), new ProductDescription("dove soap"), new ProductPrice(new Price(5m), new Price(2m)),
            new AvailableQuantity(20), [new Tag("soap"), new Tag("body")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce220")), new ProductName("colgate"), new ProductDescription("colgate toothpaste"), new ProductPrice(new Price(3m), new Price(1m)), new AvailableQuantity(30),
            [new Tag("toothpaste"), new Tag("teeth")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce221")), new ProductName("dettol"), new ProductDescription("dettol handwash"), new ProductPrice(new Price(4m), new Price(2m)), new AvailableQuantity(40),
            [new Tag("handwash"), new Tag("hands")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce222")), new ProductName("tide"), new ProductDescription("tide detergent"), new ProductPrice(new Price(8m), new Price(4m)), new AvailableQuantity(50),
            [new Tag("detergent"), new Tag("clothes")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce223")), new ProductName("pepsi"), new ProductDescription("pepsi drink"), new ProductPrice(new Price(2m), new Price(1m)), new AvailableQuantity(60),
            [new Tag("drink"), new Tag("beverage")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce224")), new ProductName("lays"), new ProductDescription("lays chips"), new ProductPrice(new Price(1m), new Price(0.5m)), new AvailableQuantity(70),
            [new Tag("chips"), new Tag("snacks")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce225")), new ProductName("kitkat"), new ProductDescription("kitkat chocolate"), new ProductPrice(new Price(3m), new Price(1.5m)), new AvailableQuantity(80),
            [new Tag("snacks"), new Tag("chocolate")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce226")), new ProductName("dairy milk"), new ProductDescription("dairy milk chocolate"), new ProductPrice(new Price(2.99m)), new AvailableQuantity(90),
            [new Tag("snacks"), new Tag("chocolate"), new Tag("drink")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce227")), new ProductName("sprite"), new ProductDescription("sprite drink"), new ProductPrice(new Price(1.21m)), new AvailableQuantity(100),
            [new Tag("drink")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce228")), new ProductName("pepsi"), new ProductDescription("pepsi cola"), new ProductPrice(new Price(1.25m)), new AvailableQuantity(100),
            [new Tag("drink")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce230")), new ProductName("nivea"), new ProductDescription("nivea soap"), new ProductPrice(new Price(5m), new Price(1m)),
            new AvailableQuantity(20), [new Tag("soap"), new Tag("body")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce231")), new ProductName("isana"), new ProductDescription("isana soap"), new ProductPrice(new Price(5m), new Price(1m)),
            new AvailableQuantity(20), [new Tag("soap"), new Tag("body")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        yield return new Product(new ProductId(new Guid("4c735c18-dcbf-4e72-b479-fa1e36dce232")), new ProductName("domol"), new ProductDescription("domol soap"), new ProductPrice(new Price(5m), new Price(1m)),
            new AvailableQuantity(20), [new Tag("soap"), new Tag("body")], [new ProductImage(new Uri("/images/catalog/408787_360_350_1709189190.png"), "dove soap")]);
        
    }
}