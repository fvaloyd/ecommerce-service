using Microsoft.AspNetCore.Http;

namespace Ecommerce.Contracts.Products;

public record CreateProductRequest
(
    string Name,
    float Price,
    int BrandId,
    int CategoryId,
    int StoreId,
    IFormFile File
);
