namespace Ecommerce.Api.Dtos.Store;

public class StoreWithProductDto
{
    public string Name { get; set; } = null!;
    public bool State { get; set; }
    public IEnumerable<string> ProductsName { get; set; } = null!;
}
