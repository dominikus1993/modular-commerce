namespace Modular.Ecommerce.Catalog.Core.UseCase.Requests;

public sealed class SearchProducts
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
}