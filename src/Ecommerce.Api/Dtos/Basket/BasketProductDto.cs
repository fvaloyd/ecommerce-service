using Ecommerce.Api.Dtos.Product;

namespace Ecommerce.Api.Dtos.Basket;

public record BasketProductDto
(
    float Total,
    IEnumerable<GetProductDto> Products
);
