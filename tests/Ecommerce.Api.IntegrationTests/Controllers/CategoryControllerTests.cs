using Ecommerce.Contracts;
using Ecommerce.Core.Entities;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class CategoryControllerTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

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
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Category.GetAll);

        var responseReaded = await response.Content.ReadAsStringAsync();

        var parseResponse = JsonConvert.DeserializeObject<IEnumerable<CategoryResponse>>(responseReaded);

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
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Category.GetById.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Category.GetById.Replace("{id}", unExistingId.ToString()));

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
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Category.GetById.Replace("{id}", validId.ToString()));

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
        var dto = new CreateCategoryRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Category.Create, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        var dto = new EditCategoryRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Category.Edit.Replace("{id}", invalidId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExistingId = 100_000_000;

        var dto = new EditCategoryRequest("test", true);
        
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Category.Edit.Replace("{id}", unExistingId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditCategory_ShouldReturnNoContent_WhenExistingCategoryIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var categoryDb = db.Categories.First();       

        var dto = new EditCategoryRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Category.Edit.Replace("{id}", categoryDb.Id.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Category.Delete.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNotFound_WhenUnExistingCategoryIdIsSending()
    {
        // Arrange
        int unExisitingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Category.Delete.Replace("{id}", unExisitingId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_WhenValidIdIsSending()
    {
        // Arrange
        var cat = new Category("test", true);
        
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        db.Categories.Add(cat);
        
        await db.SaveChangesAsync();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Category.Delete.Replace("{id}", cat.Id.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}