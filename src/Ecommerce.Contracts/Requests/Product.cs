using Microsoft.AspNetCore.Http;

namespace Ecommerce.Contracts.Requests;

public record CreateProductRequest(
    string Name,
    float Price,
    int BrandId,
    int CategoryId,
    int StoreId,
    IFormFile File
);

public record EditProductRequest(
    string Name,
    float Price,
    int BrandId,
    int CategoryId
);
