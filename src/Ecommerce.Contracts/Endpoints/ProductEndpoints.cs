namespace Ecommerce.Contracts.Endpoints;

public static class ProductEndpoints
{
    private const string root = "products";
    public const string GetAllProducts = root;
    public const string GetProductById = root+"/";
    public const string CreateProduct = root;
    public const string EditProduct = root+"/";
    public const string DeleteProduct = root+"/";
}