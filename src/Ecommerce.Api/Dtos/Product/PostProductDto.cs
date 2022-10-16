namespace Ecommerce.Api.Dtos.Product;

public record PostProductDto
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId,
    int StoreId,
    IFormFile File
);
