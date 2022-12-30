using Ecommerce.Api.Dtos.Product;

namespace Ecommerce.Api.Dtos.Basket;

public record BasketResponse(
    float Total,
    IEnumerable<ProductResponse> Products);