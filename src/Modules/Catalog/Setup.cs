using Catalog.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            new ProductPrice(new Price(10m), new Price(5m)), new AvailableQuantity(10), [new Tag("cream"), new Tag("face")]);
    }
}