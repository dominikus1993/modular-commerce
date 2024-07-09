using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Modular.Ecommerce.Catalog.Api.Responses;
using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.Model;
using Modular.Ecommerce.Catalog.Core.UseCase;
using Modular.Ecommerce.Catalog.Core.UseCase.Requests;

namespace Modular.Ecommerce.Catalog.Api;

public static class Endpoints
{
    public static WebApplication MapCatalog(this WebApplication app, string path = "/catalog")
    {
        var builder = app.MapGroup(path);
        builder.MapGet("/", SearchProducts)
            .WithName("GetCatalog");
        builder.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById");
        return app;
    }

    private static async Task<Results<Ok<ProductDetailsDto>, NotFound>> GetProductById(Guid id, [FromServices]GetProductByIdUseCase useCase, CancellationToken cancellationToken)
    {
        var result = await useCase.Execute(new GetProductById(ProductId.From(id)), cancellationToken);
        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<PagedProductsResult>, NoContent, BadRequest<ProblemDetails>>> SearchProducts([AsParameters]SearchProductsRequest request, [FromServices]IValidator<SearchProductsRequest> validator, CancellationToken cancellationToken)
    {

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.BadRequest(new ProblemDetails()
            {
                Detail = validationResult.ToString(),
                Title = "Validation error",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        var query = new SearchProducts()
        {
            PriceFrom = request.PriceFrom,
            PriceTo = request.PriceTo,
            Query = request.Query,
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 12
        };

        var result = await request.ProductsUseCase.SearchProducts(query, cancellationToken);

        if (result is { Count: 0 })
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(new PagedProductsResult(result.Data, result.Count, result.Total));
    }
}