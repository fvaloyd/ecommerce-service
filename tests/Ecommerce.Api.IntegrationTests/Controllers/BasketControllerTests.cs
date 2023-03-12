using Ecommerce.Contracts.Baskets;
using Ecommerce.Contracts.Endpoints;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class BasketControllerTests
{
    const string ApiRoot = "api/";
    const string AddProductPath = $"{ApiRoot}{BasketEndpoints.AddProduct}";
    const string RemoveProductPath = $"{ApiRoot}{BasketEndpoints.RemoveProduct}";
    const string DecreaseproductPath = $"{ApiRoot}{BasketEndpoints.DecreaseProduct}";
    const string GetProductsPath = $"{ApiRoot}{BasketEndpoints.GetProducts}";
    const string IncreaseProductPath = $"{ApiRoot}{BasketEndpoints.IncreaseProduct}";

    readonly BaseIntegrationTest _baseIntegrationTest;

    public BasketControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task AddProduct_ShouldReturnBadRequest_WhenInvalidProductIdIsSent()
    {
        // Arrange
        var invalidProductId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + invalidProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnOk_WhenValidProductIdIsSent()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.ProductStores.Select(ps => ps.Product.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(RemoveProductPath + validProductId);
    
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnBadRequest_WhenProductIsAlreadyInTheBasket()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + validProductId, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnBadRequest_WhenTheBasketDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).OrderByDescending(l => l).First();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(DecreaseproductPath + productId, null!);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnOk_WhenTheBasketHaveTheProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var product = db.Products.First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + product.Id, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(DecreaseproductPath + product.Id, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WhenUserHasProductsInBasket()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var product = db.Products.First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + product.Id, null);

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + (product.Id + 1), null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(GetProductsPath);

        string responseReadedAsString = await response.Content.ReadAsStringAsync();

        BasketResponse basketProducts = JsonConvert.DeserializeObject<BasketResponse>(responseReadedAsString);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        basketProducts.Should().NotBeNull();

        basketProducts.Products.Count().Should().BeGreaterThanOrEqualTo(2);

        basketProducts.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnNotFound_WhenTheBasketDoesnHaveTheProduct()
    {
        // Arrange
        int invalidProductId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(IncreaseProductPath + invalidProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncreaseProduct_ShoulReturnOk_WhenBasketHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = db.Products.Select(p => p.Id).First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + validProductId, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(IncreaseProductPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnNotFound_WhenTheUserDoesntHaveTheSpecificProductInBasket()
    {
        // Arrange
        int unExistingProductId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(RemoveProductPath + unExistingProductId);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnOk_WhenTheUserHaveTheSpecificProductInBasket()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var product = db.Products.First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(AddProductPath + product.Id, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(RemoveProductPath + product.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}