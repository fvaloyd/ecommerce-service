using Ecommerce.Api.Dtos.Product;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class StoreControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;
    string endpointPath = "api/store/";

    public StoreControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAListOfStores()
    {
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + "getall");
        var readResponse = await response.Content.ReadAsStringAsync();
        var storeList = JsonConvert.DeserializeObject<IEnumerable<Store>>(readResponse);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        storeList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnBadRequest()
    {
        var invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_WithUnExistingId_ShouldReturnNotFound()
    {
        var unExistinId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{unExistinId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkWithStore()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{storeId}");
        var readResponse = await response.Content.ReadAsStringAsync();
        var store = JsonConvert.DeserializeObject<Store>(readResponse);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        store.Id.Should().Be(storeId);
    }

    [Fact]
    public async Task Create_WithInvalidStoreName_ShouldReturnBadRequest()
    {
        string invalidStoreName = "";
        PostStoreDto dto = new(invalidStoreName, true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnRedirect()
    {
        PostStoreDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Edit_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;
        PutStoreDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{invalidId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Edit_WithUnExistingId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;
        PutStoreDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{unExistingId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Edit_WithValidId_ShouldReturnNoContent()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        PutStoreDto dto = new("test", true);

        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{storeId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WithUnExistingId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{unExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnNoContent()
    {
        PostStoreDto dto = new("test", true);
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        _ = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{storeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task IncreaseProduct_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={invalidId}&productId={invalidId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IncreaseProduct_WithNoRelationProductStore_ShouldReturnBadRequest()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();
        int unExistingId = 100_100_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={storeId}&productId={unExistingId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IncreaseProduct_WithValidIds_ShouldReturnOk()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();
        var productId = db.Products.Select(p => p.Id).First();

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={storeId}&productId={productId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DecreaseProduct_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={invalidId}&productId={invalidId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProduct_WithNoRelationProductStore_ShouldReturnBadRequest()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();
        int unExistingId = 100_100_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={storeId}&productId={unExistingId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProduct_WithValidIds_ShouldReturnOk()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();
        var productId = db.Products.Select(p => p.Id).First();

        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={storeId}&productId={productId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreWithProducts_WithInvalidId_ShouldReturnBadRequest()
    {
        int invalidId = 0;

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{invalidId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreWithProducts_WithUnExistingId_ShouldReturnNotFound()
    {
        int unExistingId = 100_000_000;

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{unExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStoreWithProducts_WithValidId_ShouldReturnOkWithTheStore()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var storeId = db.Stores.Select(s => s.Id).First();

        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{storeId}");
        var readResponse = await response.Content.ReadAsStringAsync();
        var store = JsonConvert.DeserializeObject<StoreWithProductDto>(readResponse);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        store.Should().NotBeNull();
        store.ProductsName.Should().NotBeNullOrEmpty();
    }
}