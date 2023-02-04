using Ecommerce.Api.Dtos.Store;
using Ecommerce.Core.Entities;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class StoreControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endpointPath = "api/store/";

    public StoreControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAListOfStores()
    {
        // Arrange
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + "getall");

        // Act
        var readResponse = await response.Content.ReadAsStringAsync();

        var storeList = JsonConvert.DeserializeObject<IEnumerable<Store>>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        storeList.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        var unExistinId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{unExistinId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"getbyid/{storeId}");

        var readResponse = await response.Content.ReadAsStringAsync();

        var store = JsonConvert.DeserializeObject<Store>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        store.Id.Should().Be(storeId);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenInvalidStoreIsSent()
    {
        // Arrange
        string invalidStoreName = "";

        CreateStoreRequest dto = new(invalidStoreName, true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ShouldReturnRedirect_WhenValidCreateStoreRequestIsSent()
    {
        // Arrange
        CreateStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Edit_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{invalidId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{unExistingId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Edit_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        EditStoreRequest dto = new("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(endpointPath + $"edit/{storeId}", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{unExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        CreateStoreRequest dto = new("test", true);

        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        _ = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endpointPath + "create", dto);

        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(endpointPath + $"delete/{storeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={invalidId}&productId={invalidId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnBadRequest_WhenTheStoreDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        int unExistingId = 100_100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={storeId}&productId={unExistingId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnOk_WhenStoreHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"increaseProduct?storeId={storeId}&productId={productId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={invalidId}&productId={invalidId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnBadRequest_WhenTheStoreDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        int unExistingId = 100_100_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={storeId}&productId={unExistingId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnOk_WhenTheStoreHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        var productId = db.Products.Select(p => p.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(endpointPath + $"decreaseProduct?storeId={storeId}&productId={productId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreWithProducts_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreWithProducts_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{unExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStoreWithProducts_ShouldReturnOk_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var storeId = db.Stores.Select(s => s.Id).First();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(endpointPath + $"GetStoreWithProducts/{storeId}");

        var readResponse = await response.Content.ReadAsStringAsync();

        var store = JsonConvert.DeserializeObject<StoreResponse>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        store.Should().NotBeNull();

        store.Products.Should().NotBeNullOrEmpty();
    }
}