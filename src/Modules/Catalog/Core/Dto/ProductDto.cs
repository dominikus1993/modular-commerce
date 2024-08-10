using Modular.Ecommerce.Catalog.Core.Model;

namespace Modular.Ecommerce.Catalog.Core.Dto;

public sealed class ProductDto
{
    public Guid ProductId { get; init; }
    
    public string Name { get; init; }
    public string Description { get; init; }

    public decimal? PromotionalPrice { get; init; }

    public decimal Price { get; init; }
    
    public int AvailableQuantity { get; init; }
    
    public IReadOnlyList<ProductImageDto> Images { get; init; }
    
    public ProductDto(Product product)
    {
        ProductId = product.Id.Value;
        Description = product.ProductDescription.Description;
        Name = product.ProductName.Name;
        Price = product.Price.CurrentPrice;
        PromotionalPrice = product.Price.PromotionalPrice;
        AvailableQuantity = product.AvailableQuantity.Value;
        Images = product.Images.Select(x => new ProductImageDto(x.Link, x.Alt)).ToArray();
    }
}