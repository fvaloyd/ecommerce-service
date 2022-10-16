namespace Ecommerce.Api.Dtos.Product;

public record PutProductDto
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId
);
