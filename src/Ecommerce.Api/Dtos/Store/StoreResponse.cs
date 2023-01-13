using Ecommerce.Api.Dtos.Product;

namespace Ecommerce.Api.Dtos.Store;

public record StoreResponse(
    Ecommerce.Core.Entities.Store Store,
    IEnumerable<ProductResponse> Products);