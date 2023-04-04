namespace Ecommerce.Contracts.Requests;

public record CreateStoreRequest(string Name, bool State);

public record EditStoreRequest(string Name, bool State);