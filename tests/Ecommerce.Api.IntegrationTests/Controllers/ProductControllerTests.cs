using Ecommerce.Api.Dtos.Product;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class ProductControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;
    string endpointPath = "api/product/";

    public ProductControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAListOfProduct()
    {
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + "getall");
        var responseRead = await response.Content.ReadAsStringAsync();
        var productList = JsonConvert.DeserializeObject<IEnumerable<GetProductDto>>(responseRead);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WithUnExistingId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{unExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkWithCorrespondingProduct()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var productId = db.Products.Select(p => p.Id).First();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endpointPath + $"getbyid/{productId}");
        var responseRead = await response.Content.ReadAsStringAsync();
        var product = JsonConvert.DeserializeObject<GetProductDto>(responseRead);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Id.Should().Be(productId);
    }

    [Fact]
    public async Task Create_WithInvalidStoreId_ShouldReturnBadRequest()
    {
        IFormFile file = new FormFile(null!, 1, 1, "test", "test");
        int invalidStoreId = 0;
        PostProductDto dto = new("test", 100f, 1, 1, invalidStoreId, file);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithValidStoreId_ShouldReturnRedirect()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();
        var brandId = db.Brands.Select(b => b.Id).First();
        var categoryId = db.Categories.Select(c => c.Id).First();
        IFormFile file = new FormFile(null!, 1, 1, "test", "test");
        PostProductDto dto = new("test", 100f, brandId, categoryId, storeId, file);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);
    }

    [Fact]
    public async Task Edit_WithInvalidId_ShouldReturnBadRequest()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var categoryId = db.Categories.Select(c => c.Id).First();
        var brandId = db.Brands.Select(b => b.Id).First();
        PutProductDto dto = new("test", 100f, brandId, categoryId);
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{invalidId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Edit_WithUnExistingId_ShouldReturnNotFound()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var categoryId = db.Categories.Select(c => c.Id).First();
        var brandId = db.Brands.Select(b => b.Id).First();
        PutProductDto dto = new("test", 100f, brandId, categoryId);
        int unExistingId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{unExistingId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Edit_WithValidId_ShouldReturnOk()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var categoryId = db.Categories.Select(c => c.Id).First();
        var brandId = db.Brands.Select(b => b.Id).First();
        var productId = db.Products.Select(p => p.Id).First();
        PutProductDto dto = new("test", 100f, brandId, categoryId);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{productId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnBadRequest()
    {
        var invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WithUnExistingId_ShouldReturnNotFound()
    {
        var unExistingId = 100_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{unExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnOK()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var productId = db.Products.Select(p => p.Id).First();

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{productId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}