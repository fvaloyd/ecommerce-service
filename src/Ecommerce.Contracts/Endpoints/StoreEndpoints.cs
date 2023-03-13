namespace Ecommerce.Contracts.Endpoints;

public static class StoreEndpoints
{
    private const string root = "store/";
    public const string GetAllStores = root+"get-all";
    public const string GetStoreById = root+"get-by-id/";
    public const string CreateStore = root+"create";
    public const string EditStore = root+"edit/";
    public const string DeleteStore = root+"delete/";
    public const string IncreaseProduct = root+"increase-product";
    public const string DecreaseProduct = root+"decrease-product";
    public const string GetStoreWithProduct = root+"get-with-product/";
    public const string GetStoreWithProductPaginated = root+"get-with-product-paginated";  
}