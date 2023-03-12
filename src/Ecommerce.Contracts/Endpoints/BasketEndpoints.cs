namespace Ecommerce.Contracts.Endpoints;

public static class BasketEndpoints
{
    private const string root = "basket/";
    public const string AddProduct = root+"add-product/";
    public const string IncreaseProduct = root+"increase-product/";
    public const string DecreaseProduct = root+"decrease-product/";
    public const string RemoveProduct = root+"remove-product/";
    public const string GetProducts = root+"get-products"; 
}