using Ecommerce.Contracts.Products;

namespace Ecommerce.Contracts.Stores;

public record StoreWithProductResponse(
    StoreResponse Store,
    IEnumerable<ProductResponse> Products);

public record StoreResponse(int Id, string Name, bool State);