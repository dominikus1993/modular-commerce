using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modular.Ecommerce.Catalog.Core.UseCase;

namespace Modular.Ecommerce.Catalog;

public static class Setup
{
    public static WebApplicationBuilder AddCatalog(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<SearchProductsUseCase>();
        builder.Services.AddTransient<GetProductByIdUseCase>();
        return builder;
    }
}