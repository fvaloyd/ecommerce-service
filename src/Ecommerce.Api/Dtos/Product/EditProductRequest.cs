namespace Ecommerce.Api.Dtos.Product;

public record EditProductRequest
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId
);
