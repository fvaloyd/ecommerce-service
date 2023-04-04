namespace Ecommerce.Contracts.Requests;

public record CreateCategoryRequest(string Name, bool State);

public record EditCategoryRequest(string Name, bool State);