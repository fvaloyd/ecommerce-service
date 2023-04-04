namespace Ecommerce.Contracts.Requests;

public record CreateBrandRequest(string Name, bool State);

public record EditBrandRequest(string Name, bool State);