using Ecommerce.Contracts.Responses;

namespace Ecommerce.Contracts.Responses;

public record BasketResponse(
    float Total,
    IEnumerable<ProductResponse> Products);