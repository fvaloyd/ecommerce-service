namespace Ecommerce.Contracts.Endpoints;

public static class CategoryEndpoints
{
    private const string root = "category/";
    public const string GetAllCategories = root+"get-all";
    public const string GetCategoryById = root+"get-by-id/";
    public const string CreateCategory = root+"create";
    public const string EditCategory = root+"edit/";
    public const string DeleteCategory = root+"delete";
}