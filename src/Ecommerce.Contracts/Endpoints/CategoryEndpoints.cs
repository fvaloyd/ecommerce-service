namespace Ecommerce.Contracts.Endpoints;

public static class CategoryEndpoints
{
    private const string root = "categories";
    public const string GetAllCategories = root;
    public const string GetCategoryById = root+"/";
    public const string CreateCategory = root;
    public const string EditCategory = root+"/";
    public const string DeleteCategory = root+"/";
}