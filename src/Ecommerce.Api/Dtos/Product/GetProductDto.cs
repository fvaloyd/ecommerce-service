namespace Ecommerce.Api.Dtos.Product;

public record GetProductDto
(
    int Id,
    string Name,
    float Price,
    string BrandName,
    string CategoryName,
    string ImageUrl
);