using Microsoft.AspNetCore.Builder;

namespace Modular.Ecommerce.Catalog;

public static class Setup
{
    public static WebApplicationBuilder AddCatalog(this WebApplicationBuilder builder)
    {
        return builder;
    }
}