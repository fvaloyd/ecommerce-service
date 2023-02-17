using Ecommerce.Api.Dtos.Product;

namespace Ecommerce.Api.Dtos.Store;

public record StoreWithProductResponse(
    StoreResponse Store,
    IEnumerable<ProductResponse> Products);

public record StoreResponse(int Id, string Name, bool State);