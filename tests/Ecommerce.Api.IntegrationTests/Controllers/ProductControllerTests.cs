using Ecommerce.Contracts;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class ProductControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;

    public ProductControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnOkWithAListOfProduct()
    {
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Product.GetAll);

        var responseRead = await response.Content.ReadAsStringAsync();

        var productList = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(responseRead);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        productList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Product.GetById.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Product.GetById.Replace("{id}", unExistingId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Product.GetById.Replace("{id}", productId.ToString()));

        var responseRead = await response.Content.ReadAsStringAsync();

        var product = JsonConvert.DeserializeObject<ProductResponse>(responseRead);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        product.Id.Should().Be(productId);
    }

    [Fact]
    public async Task EditProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        var dto = new EditProductRequest("test", 100f, brandId, categoryId);

        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Product.Edit.Replace("{id}", invalidId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditProduct_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        var dto = new EditProductRequest("test", 100f, brandId, categoryId);

        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Product.Edit.Replace("{id}", unExistingId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditProduct_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var categoryId = db.Categories.Select(c => c.Id).First();

        var brandId = db.Brands.Select(b => b.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        var dto = new EditProductRequest("test", 100f, brandId, categoryId);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Product.Edit.Replace("{id}", productId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Product.Delete.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        var unExistingId = 100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Product.Delete.Replace("{id}", unExistingId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOK_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Product.Delete.Replace("{id}", productId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}