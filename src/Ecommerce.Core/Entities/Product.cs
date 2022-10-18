namespace Ecommerce.Core.Entities;

public class Product : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public float Price { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public void ChangePrice(float newPrice)
    {
        Price = newPrice;
    }

    public Product(){}
    public Product(
        string name,
        float price,
        int brandId,
        int categoryId,
        string imageUrl)
    {
        Name = name;
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
        ImageUrl = imageUrl;
    }
}
