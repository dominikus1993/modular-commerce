using Catalog.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    }
}