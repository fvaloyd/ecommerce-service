namespace Ecommerce.Api.Dtos.Product;

public record CreateProductRequest
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId,
    int StoreId,
    IFormFile File
);
