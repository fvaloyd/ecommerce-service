namespace Ecommerce.Contracts.Endpoints;

public static class ProductEndpoints
{
    private const string root = "product/";
    public const string GetAllProducts = root+"get-all";
    public const string GetProductById = root+"get-by-id/";
    public const string CreateProduct = root+"create";
    public const string EditProduct = root+"edit/";
    public const string DeleteProduct = root+"delete/";
}