using Ecommerce.Api.Dtos.Store;
using Ecommerce.Application.Common.Models;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class StoreControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;

    const string endpointPath = "api/store/";
    const string GetStoreWithProductPath = $"{endpointPath}GetStoreWithProduct/";
    const string DecreaseProductInStorePath = $"{endpointPath}DecreaseProductInStore?";
    const string IncreaseProductInStorePath = $"{endpointPath}IncreaseProductInStore?";
    const string DeleteStorePath = $"{endpointPath}DeleteStore/";
    const string EditStorePath = $"{endpointPath}EditStore/";
    const string CreateStorePath = $"{endpointPath}CreateStore/";
    const string GetStoreByIdPath = $"{endpointPath}GetStoreById/";
    const string GetAllStoresPath = $"{endpointPath}GetAllStores/";
    const string GetStoreWithProductPaginated = $"{endpointPath}GetStoreWithProductPaginated";


    public StoreControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllStores_ShouldReturnOkWithAListOfStores()
    {
        // Arrange
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetAllStoresPath);

        // Act
        var readResponse = await response.Content.ReadAsStringAsync();

        var storeList = JsonConvert.DeserializeObject<IEnumerable<Store>>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        storeList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStoreById_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreByIdPath + invalidId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreById_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        var unExistinId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreByIdPath + unExistinId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStoreById_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreByIdPath + storeId);

        var readResponse = await response.Content.ReadAsStringAsync();

        var store = JsonConvert.DeserializeObject<Store>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        store.Id.Should().Be(storeId);
    }

    [Fact]
    public async Task CreateStore_ShouldReturnBadRequest_WhenInvalidStoreIsSent()
    {
        // Arrange
        string invalidStoreName = "";

        CreateStoreRequest dto = new(invalidStoreName, true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(CreateStorePath, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStore_ShouldReturnRedirect_WhenValidCreateStoreRequestIsSent()
    {
        // Arrange
        CreateStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(CreateStorePath, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task EditStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(EditStorePath + invalidId, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditStore_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(EditStorePath + unExistingId, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditStore_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(EditStorePath + storeId, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(DeleteStorePath + invalidId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnBadRequest_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(DeleteStorePath + unExistingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        CreateStoreRequest dto = new("test", true);

        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        _ = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(CreateStorePath, dto);

        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(DeleteStorePath + storeId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task IncreaseProductInStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(IncreaseProductInStorePath +  $"storeId={invalidId}&productId={invalidId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IncreaseProductInStore_ShouldReturnBadRequest_WhenTheStoreDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        int unExistingId = 100_100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(IncreaseProductInStorePath + $"storeId={storeId}&productId={unExistingId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncreaseProductInProduct_ShouldReturnOk_WhenStoreHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(IncreaseProductInStorePath + $"storeId={storeId}&productId={productId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DecreaseProductInStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(DecreaseProductInStorePath + $"storeId={invalidId}&productId={invalidId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProductInStore_ShouldReturnBadRequest_WhenTheStoreDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        int unExistingId = 100_100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(DecreaseProductInStorePath + $"storeId={storeId}&productId={unExistingId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecreaseProductInStore_ShouldReturnOk_WhenTheStoreHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(DecreaseProductInStorePath + $"storeId={storeId}&productId={productId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreWithProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreWithProductPath + invalidId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreWithProduct_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreWithProductPath + unExistingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStoreWithProduct_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(GetStoreWithProductPath + storeId);

        var readResponse = await response.Content.ReadAsStringAsync();

        var store = JsonConvert.DeserializeObject<StoreWithProductResponse>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        store.Should().NotBeNull();

        store.Products.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStoreWithProductPaginated_ShouldReturnOk_WhenValidPaginationIsSent()
    {
        // Arrange
        Pagination pagination = new(3, 1);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(GetStoreWithProductPaginated + $"?pageSize={pagination.PageSize}&pageNumber={pagination.PageNumber}");

        var readResponse = await response.Content.ReadAsStringAsync();

        var storeWithProduct = JsonConvert.DeserializeObject<StoreWithProductResponse>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        storeWithProduct.Should().NotBeNull();

        storeWithProduct.Products.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetStoreWithProductPaginated_ShouldReturnNotFound_WhenInValidPaginationIsSent()
    {
        // Arrange
        Pagination pagination = new(100_000, 100);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(GetStoreWithProductPaginated + $"?pageSize={pagination.PageSize}&pageNumber={pagination.PageNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}