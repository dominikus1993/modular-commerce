using Modular.Ecommerce.Catalog.Core.Model;
using OpenSearch.Client;

namespace Modular.Ecommerce.Catalog.Infrastructure.Model;

public sealed class OpenSearchProductImage
{
    public Uri Link { get; set; }
    public string Alt { get; set; }
}

[OpenSearchType(IdProperty = nameof(ProductId))]
internal sealed class OpenSearchProduct
{
    public Id ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public decimal? PromotionalPrice { get; set; }

    public decimal Price { get; set; }
    
    public decimal SalePrice { get; set; } 
    
    public int AvailableQuantity { get; set; }
    
    public IReadOnlyList<string>? Tags { get; set; }

    public IReadOnlyList<OpenSearchProductImage> Images { get; set; } = [];
    
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;

    public OpenSearchProduct()
    {
        
    }

    public OpenSearchProduct(Product product)
    {
        ProductId = product.Id.Value;
        Description = product.ProductDescription.Description;
        Name = product.ProductName.Name;
        AvailableQuantity = product.AvailableQuantity.Value;
        Price = product.Price.CurrentPrice;
        PromotionalPrice = product.Price.PromotionalPrice;
        Tags = product.Tags?.Select(x => x.Name).ToArray();
        SalePrice = PromotionalPrice ?? Price;
        Images = product.Images.Select(x => new OpenSearchProductImage
        {
            Alt = x.Alt,
            Link = x.Link
        }).ToArray();
    }

    public Product ToProduct()
    {
        var tags = Tags?.Select(tag => new Tag(tag)) ?? [];
        Guid id = Guid.Parse(ProductId.ToString());
        return new Product(id, new ProductName(Name), new ProductDescription(Description),
            new ProductPrice(Price, PromotionalPrice), new AvailableQuantity(AvailableQuantity), [..tags], [..Images.Select(x => new ProductImage(x.Link, x.Alt))]);
    }
}