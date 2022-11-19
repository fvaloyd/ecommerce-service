using Ecommerce.Api.Dtos.Category;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class CategoryControllerTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/category/";
    public CategoryControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnOkAndAListOfCategories()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var categoriesDb = db.Categories.ToList();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + "getall");
        var responseReaded = await response.Content.ReadAsStringAsync();
        var parseResponse = JsonConvert.DeserializeObject<IEnumerable<Category>>(responseReaded);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        parseResponse.Should().NotBeNull();
        parseResponse.Count().Should().Be(categoriesDb.Count());
    }

    [Fact]
    public async Task GetCategoryById_WithInvalidIdShouldReturnBadRequest()
    {
        int invalidId = 0;
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{invalidId.ToString()}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoryById_WithUnExistedId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{unExistingId.ToString()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategoryById_WithValidId_ShouldReturnOkAndASingleCategory()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        int validId = db.Categories.Select(c => c.Id).First();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{validId.ToString()}");
        var responseReaded = await response.Content.ReadAsStringAsync();
        var parseResponse = JsonConvert.DeserializeObject<Category>(responseReaded);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        parseResponse.Should().NotBeNull();
        parseResponse.Id.Should().Be(validId);
    }

    // [Fact]
    // public async Task CreateCategory_WithExistingCategory_ShouldReturnBadRequest()
    // {
    //     using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
    //     var categoryDb = db.Categories.First();       
    //     PostCategoryDto dto = new(categoryDb.Name, categoryDb.State);

    //     var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath + "create", dto);

    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }

    [Fact]
    public async Task CreateCategory_WithValidCategory_ShouldReturnBadRequest()
    {
        PostCategoryDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath + "create", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task EditCategory_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;
        PutCategoryDto dto = new("test", true);
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{invalidId.ToString()}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditCategory_WithUnExistingId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;
        PutCategoryDto dto = new("test", true);
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{unExistingId.ToString()}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditCategory_WithValidId_ShouldReturnNoContent()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var categoryDb = db.Categories.First();       
        PutCategoryDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{categoryDb.Id.ToString()}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_WithUnExistingId_ShouldReturnBadRequest()
    {
        int unExisitingId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{unExisitingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_WithValidId_ShouldReturnNoContent()
    {
        Category cat = new("test", true);
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        db.Categories.Add(cat);
        await db.SaveChangesAsync();

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{cat.Id.ToString()}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}