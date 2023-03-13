namespace Ecommerce.Contracts.Endpoints;

public static class BrandEndpoints
{
    private const string root = "brand/";
    public const string GetAllBrands = root+"get-all";
    public const string GetBrandById = root+"get-by-id/";
    public const string CreateBrand = root+"create";
    public const string EditBrand = root+"edit/";
    public const string DeleteBrand = root+"delete/";
}