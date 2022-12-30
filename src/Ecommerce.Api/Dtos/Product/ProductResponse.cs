namespace Ecommerce.Api.Dtos.Product;

public record ProductResponse
(
    int Id,
    string Name,
    float Price,
    string BrandName,
    string CategoryName,
    string ImageUrl
);