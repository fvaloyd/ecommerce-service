using Ecommerce.Contracts;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class BrandController
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    public BrandController(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllBrands_ShouldReturnAListOfBrands()
    {
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Brand.GetAll);
    
        var responseReaded = await response.Content.ReadAsStringAsync();
        
        var listOfBrands = JsonConvert.DeserializeObject<List<BrandResponse>>(responseReaded);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        listOfBrands.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBrandById_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Brand.GetById.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBrandById_ShouldReturnNotFound_WhenUnExistingBrandIdIsSending()
    {
        // Arrange
        int randomId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Brand.GetById.Replace("{id}", randomId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBrandById_ShouldReturnOkWithASingleBrand_WhenValidIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var dbBrand = db.Brands.First();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Brand.GetById.Replace("{id}", dbBrand.Id.ToString()));
        
        var responseReaded = await response.Content.ReadAsStringAsync();
        
        var requestBrand = JsonConvert.DeserializeObject<Ecommerce.Core.Entities.Brand>(responseReaded);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        requestBrand.Should().BeEquivalentTo(dbBrand);
    }

    [Fact]
    public async Task CreateBrand_ShouldRedirectToRoute_WhenValidBrandIsSending()
    {
        // Arrange
        var validDto = new CreateBrandRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Brand.Create, validDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Found);
    }

    [Fact]
    public async Task EditBrand_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        var dto = new EditBrandRequest(default!, default);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Brand.Edit.Replace("{id}", invalidId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditBrand_ShouldReturnNotFound_WhenUnExistingBrandIdIsSending()
    {
        // Arrange
        int randomId = 100_000_000;

        var dto = new EditBrandRequest("", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Brand.Edit.Replace("{id}", randomId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditBrand_ShouldReturnNoContent_WhenValidIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var dbBrand = db.Brands.First();

        var brandDto = new EditBrandRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Brand.Edit.Replace("{id}", dbBrand.Id.ToString()), brandDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBrand_ShouldReturnBadRequest_WhenInvalidIdIsSending()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Brand.Delete.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBrand_ShouldReturnNotFound_WhenUnExistingBrandIdIsSending()
    {
        // Arrange
        int randomId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Brand.Delete.Replace("{id}", randomId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBrand_ShouldReturnNoContent_WhenValidBrandIdIsSending()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var dbBrand = db.Brands.OrderBy(b => b).Last();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Brand.Delete.Replace("{id}", dbBrand.Id.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}