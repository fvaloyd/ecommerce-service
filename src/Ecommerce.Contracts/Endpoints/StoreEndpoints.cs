namespace Ecommerce.Contracts.Endpoints;

public static class StoreEndpoints
{
    private const string root = "store";
    public const string GetAllStores = root;
    public const string GetStoreById = root+"/";
    public const string CreateStore = root;
    public const string EditStore = root+"/";
    public const string DeleteStore = root+"/";
    public const string IncreaseProduct = root+"/increase-product";
    public const string DecreaseProduct = root+"/decrease-product";
    public const string GetStoreWithProduct = root+"/get-with-product/";
    public const string GetStoreWithProductPaginated = root+"/get-with-product-paginated";  
}