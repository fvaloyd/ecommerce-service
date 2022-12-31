namespace Ecommerce.Api.Dtos.Store;

public record StoreResponse(
    string Name,
    bool State,
    IEnumerable<string> ProductsName);