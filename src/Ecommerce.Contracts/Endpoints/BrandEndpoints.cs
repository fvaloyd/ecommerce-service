namespace Ecommerce.Contracts.Endpoints;

public static class BrandEndpoints
{
    private const string root = "brands";
    public const string GetAllBrands = root;
    public const string GetBrandById = root+"/";
    public const string CreateBrand = root;
    public const string EditBrand = root+"/";
    public const string DeleteBrand = root+"/";
}