namespace Ecommerce.Contracts.Products;

public record EditProductRequest
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId
);
