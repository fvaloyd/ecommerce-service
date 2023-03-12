namespace Ecommerce.Contracts.Products;

public record ProductResponse
(
    int Id,
    string Name,
    float Price,
    string BrandName,
    string CategoryName,
    string ImageUrl
);