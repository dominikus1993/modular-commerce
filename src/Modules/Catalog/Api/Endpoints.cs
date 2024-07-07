using Microsoft.AspNetCore.Builder;

namespace Modular.Ecommerce.Catalog.Api;

public static class Endpoints
{
    public static WebApplication MapCatalog(this WebApplication app, string path = "/catalog")
    {
        var builder = app.MapGroup(path);
        builder.MapGet("/", () => new[]
            {
                new { Id = 1, Name = "Product 1" },
                new { Id = 2, Name = "Product 2" },
                new { Id = 3, Name = "Product 3" },
            })
            .WithName("GetCatalog");

        return app;
    }
}