using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class BrandController
{
    readonly BaseIntegrationTest _baseIntegrationTest;
    readonly string endPointPath = "api/brand/";
    public BrandController(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllBrands_ShouldReturnAListOfBrands()
    {
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + "getall");
        var responseReaded = await response.Content.ReadAsStringAsync();
        List<Brand> listOfBrands = JsonConvert.DeserializeObject<List<Brand>>(responseReaded);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        listOfBrands.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBrandById_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"getbyid/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBrandById_WithNoBrandWithGiveItId_ShouldReturnNotFound()
    {
        int randomId = 100_000_000;

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"getbyid/{randomId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBrandById_WithValidId_ShouldReturnOkWithSpecifigBrand()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var dbBrand = db.Brands.First();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath + $"getbyid/{dbBrand.Id}");
        var responseReaded = await response.Content.ReadAsStringAsync();
        var requestBrand = JsonConvert.DeserializeObject<Ecommerce.Core.Entities.Brand>(responseReaded);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        requestBrand.Should().BeEquivalentTo(dbBrand);
    }

    [Fact]
    public async Task CreateBrand_WithValidBrand_ShouldRedirectToRoute()
    {
        CreateBrandRequest validDto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath + "create", validDto);

        response.StatusCode.Should().Be(HttpStatusCode.Found);
    }

    [Fact]
    public async Task EditBrand_InvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;
        EditBrandRequest dto = new(default!, default);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"edit/{invalidId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditBrand_WithNoBrandWithSpecificId_ShouldReturnNotFound()
    {
        int randomId = 100_000_000;
        EditBrandRequest dto = new("", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"edit/{randomId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditBrand_WithValidId_ShouldReturnNoContent()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        Brand dbBrand = db.Brands.First();
        EditBrandRequest brandDto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endPointPath + $"edit/{dbBrand.Id}", brandDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteBrand_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"delete/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBrand_WithNoBrandWithSpecificId_ShouldReturnNotFound()
    {
        int randomId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"delete/{randomId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBrand_WithValidId_ShouldReturnNoContent()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var dbBrand = db.Brands.OrderBy(b => b).Last();

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endPointPath + $"delete/{dbBrand.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}