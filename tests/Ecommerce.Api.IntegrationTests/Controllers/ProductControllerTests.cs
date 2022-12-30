using Ecommerce.Api.Dtos.Product;

using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class ProductControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endpointPath = "api/product/";

    public ProductControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAListOfProduct()
    {
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + "getall");

        var responseRead = await response.Content.ReadAsStringAsync();

        var productList = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(responseRead);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        productList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{unExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{productId}");

        var responseRead = await response.Content.ReadAsStringAsync();

        var product = JsonConvert.DeserializeObject<ProductResponse>(responseRead);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        product.Id.Should().Be(productId);
    }

    [Fact]
    public async Task Edit_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        EditProductRequest dto = new("test", 100f, brandId, categoryId);

        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{invalidId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        EditProductRequest dto = new("test", 100f, brandId, categoryId);

        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{unExistingId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Edit_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        EditProductRequest dto = new("test", 100f, brandId, categoryId);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{productId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        var unExistingId = 100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{unExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnOK_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}