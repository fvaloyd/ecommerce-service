using Ecommerce.Core.Common;

namespace Ecommerce.Core.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public float Price { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string ImageUrl { get; private set; } = null!;

    public void ChangePrice(float newPrice)
    {
        if (newPrice < 1) throw new ArgumentException("The price could not be less than 1");
        
        Price = newPrice;
    }

    public void SetName(string name)
    {
        if (name is null) throw new ArgumentNullException();
        if (name.Length < 1) throw new ArgumentException("The Name could not have a length less than 1");

        Name = name;
    }
    public void SetImage(string imageUrl)
    {
        if (imageUrl is null) throw new ArgumentNullException();

        Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? result);

        if (result is null || !( (result!.Scheme == Uri.UriSchemeHttp) || (result.Scheme == Uri.UriSchemeHttps) ))
        {
            throw new ArgumentException("The ImageUrl is invalid");
        }

        ImageUrl = result.AbsoluteUri;
    }

    public Product(){}
    public Product(
        string name,
        float price,
        int brandId,
        int categoryId,
        string imageUrl)
    {
        SetName(name);
        Price = price;
        BrandId = brandId;
        CategoryId = categoryId;
        SetImage(imageUrl);
    }
}
