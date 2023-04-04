using Ecommerce.Contracts;
using Ecommerce.Core.Entities;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;
using Ecommerce.Application.Common.Models;

using System.Text;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class StoreControllerTests
{
    private readonly BaseIntegrationTest _baseIntegrationTest;

    public StoreControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllStores_ShouldReturnOkWithAListOfStores()
    {
        // Arrange
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetAll);

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
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetById.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreById_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        var unExistinId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetById.Replace("{id}", unExistinId.ToString()));

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
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetById.Replace("{id}", storeId.ToString()));

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

        var dto = new CreateStoreRequest(invalidStoreName, true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Store.Create, dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStore_ShouldReturnRedirect_WhenValidCreateStoreRequestIsSent()
    {
        // Arrange
        var dto = new CreateStoreRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Store.Create, dto);

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
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Store.Edit.Replace("{id}", invalidId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditStore_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        var dto = new EditStoreRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Store.Edit.Replace("{id}", unExistingId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EditStore_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        
        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        var dto = new EditStoreRequest("test", true);

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PutAsJsonAsync(ApiRoutes.Store.Edit.Replace("{id}", storeId.ToString()), dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Store.Delete.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnBadRequest_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Store.Delete.Replace("{id}", unExistingId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnNoContent_WhenValidIdIsSent()
    {
        // Arrange
        var dto = new CreateStoreRequest("test", true);

        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        _ = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Store.Create, dto);

        var storeId = db.Stores.OrderBy(s => s).Select(s => s.Id).Last();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.DeleteAsync(ApiRoutes.Store.Delete.Replace("{id}", storeId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task IncreaseProductInStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        var uri = new StringBuilder(ApiRoutes.Store.IncreaseProduct)
                        .Append($"?productId={invalidId}")
                        .ToString();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri.Replace("{id}", invalidId.ToString()), null);

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

        var uri = new StringBuilder(ApiRoutes.Store.IncreaseProduct)
                .Append($"?productId={unExistingId}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri.Replace("{id}", storeId.ToString()), null);

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

        var uri = new StringBuilder(ApiRoutes.Store.IncreaseProduct)
                .Append($"?productId={productId}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri.Replace("{id}", storeId.ToString()), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DecreaseProductInStore_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        var uri = new StringBuilder(ApiRoutes.Store.DecreaseProduct)
                .Append($"?productId={invalidId}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri.Replace("{id}", invalidId.ToString()), null);

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

        var uri = new StringBuilder(ApiRoutes.Store.IncreaseProduct)
                .Append($"?productId={unExistingId}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri.Replace("{id}", storeId.ToString()), null);

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

        var uri = new StringBuilder(ApiRoutes.Store.IncreaseProduct)
                .Append($"?productId={productId}")
                .ToString()
                .Replace("{id}", storeId.ToString());

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsync(uri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStoreWithProduct_ShouldReturnBadRequest_WhenInvalidIdIsSent()
    {
        // Arrange
        int invalidId = 0;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetStoreWithProduct.Replace("{id}", invalidId.ToString()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStoreWithProduct_ShouldReturnNotFound_WhenUnExistingIdIsSent()
    {
        // Arrange
        int unExistingId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetStoreWithProduct.Replace("{id}", unExistingId.ToString()));

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
        var response = await _baseIntegrationTest.AdminUserHttpClient.GetAsync(ApiRoutes.Store.GetStoreWithProduct.Replace("{id}", storeId.ToString()));

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
        var pagination = new Pagination(3, 1);

        var uri = new StringBuilder(ApiRoutes.Store.GetStoreProductsPaginated)
                .Append($"?pageSize={pagination.PageSize}&pageNumber={pagination.PageNumber}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(uri);

        var readResponse = await response.Content.ReadAsStringAsync();

        var paginatedProducts = JsonConvert.DeserializeObject<PaginatedList<ProductResponse>>(readResponse);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        paginatedProducts.Should().NotBeNull();

        paginatedProducts.Items.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetStoreWithProductPaginated_ShouldReturnNotFound_WhenInValidPaginationIsSent()
    {
        // Arrange
        var pagination = new Pagination(100_000, 100);

        var uri = new StringBuilder(ApiRoutes.Store.GetStoreProductsPaginated)
                .Append($"?pageSize={pagination.PageSize}&pageNumber={pagination.PageNumber}")
                .ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(uri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}