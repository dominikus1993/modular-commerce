using FluentValidation;
using Modular.Ecommerce.Catalog.Core.Dto;
using Modular.Ecommerce.Catalog.Core.UseCase;

namespace Modular.Ecommerce.Catalog.Api.Responses;

public sealed class SearchProductsRequestValidator : AbstractValidator<SearchProductsRequest>
{
    public SearchProductsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Invalid page number");
        RuleFor(x => x.PageSize).Must(x => x is > 0 and < 100).WithMessage("Invalid page size");
        RuleFor(x => x.PriceFrom).GreaterThanOrEqualTo(0).WithMessage("Invalid price from");
        RuleFor(x => x.PriceTo).GreaterThanOrEqualTo(0).WithMessage("Invalid price to");
        RuleFor(x => x.PriceTo).GreaterThanOrEqualTo(x => x.PriceFrom).When(x => x.PriceTo.HasValue).WithMessage("Invalid price range");
        RuleFor(x => x.Query).Length(1, 100).When(x => !string.IsNullOrEmpty(x.Query)).WithMessage("Query is too long");
    }
}

public sealed class SearchProductsRequest
{

    public int? Page { get; init; } = 1;
    public int? PageSize { get; init; } = 12;
    public string? Query { get; init; }
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public SearchProductsUseCase ProductsUseCase { get; set; }

    public override string ToString()
    {
        return $"{nameof(Page)}: {Page}, {nameof(PageSize)}: {PageSize}, {nameof(Query)}: {Query}, {nameof(PriceFrom)}: {PriceFrom}, {nameof(PriceTo)}: {PriceTo}";
    }
}

public sealed record PagedProductsResult(IEnumerable<ProductDto> Products, uint Count, uint Total);