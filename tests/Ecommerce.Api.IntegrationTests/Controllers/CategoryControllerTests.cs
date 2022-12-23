using Ecommerce.Api.Dtos.Category;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class CategoryControllerTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/category/";

    public CategoryControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnOkAndAListOfCategories()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoriesDb = db.Categories.ToList();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + "getall");

        var responseReaded = await response.Content.ReadAsStringAsync();

        var parseResponse = JsonConvert.DeserializeObject<IEnumerable<Category>>(responseReaded);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        parseResponse.Should().NotBeNull();

        parseResponse.Count().Should().Be(categoriesDb.Count);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{unExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnOkAndASingleCategory_WhenValidIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validId = db.Categories.Select(c => c.Id).First();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"GetById/{validId}");

        var responseReaded = await response.Content.ReadAsStringAsync();

        var parseResponse = JsonConvert.DeserializeObject<Category>(responseReaded);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        parseResponse.Should().NotBeNull();

        parseResponse.Id.Should().Be(validId);
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnRedirect_WhenValidCategoryIsSending()
    {
        // Arrange
        CreateCategoryRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath + "create", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        EditCategoryRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{invalidId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExistingId = 100_000_000;

        EditCategoryRequest dto = new("test", true);
        
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{unExistingId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnNoContent_WhenExistingCategoryIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var categoryDb = db.Categories.First();       

        EditCategoryRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"Edit/{categoryDb.Id.ToString()}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExisitingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{unExisitingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_WhenValidIdIsSending()
    {
        // Arrange
        Category cat = new("test", true);
        
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        db.Categories.Add(cat);
        
        await db.SaveChangesAsync();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"Delete/{cat.Id.ToString()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}