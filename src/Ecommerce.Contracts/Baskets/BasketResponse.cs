using Ecommerce.Contracts.Products;

namespace Ecommerce.Contracts.Baskets;

public record BasketResponse(
    float Total,
    IEnumerable<ProductResponse> Products);