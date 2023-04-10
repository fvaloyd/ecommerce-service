using Ecommerce.Contracts;
using Ecommerce.Contracts.Responses;

using System.Text;

namespace Ecommerce.Api.IntegrationTests.Controllers;

[Collection("BaseIntegrationTestCollection")]
public class BasketControllerTests
{
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

        var uri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={invalidProductId}")
                        .ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(uri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnOk_WhenValidProductIdIsSent()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var validProductId = Db.ProductStores.Select(ps => ps.Product.Id).First();

        var removeProductUri = new StringBuilder(ApiRoutes.Basket.RemoveProduct)
                        .Append($"?productId={validProductId}")
                        .ToString();
        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={validProductId}")
                        .ToString();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(removeProductUri);
    
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnBadRequest_WhenProductIsAlreadyInTheBasket()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var validProductId = Db.Products.Select(p => p.Id).First();

        var uri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={validProductId}")
                        .ToString();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(uri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(uri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnBadRequest_WhenTheBasketDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).OrderByDescending(l => l).First().ToString();

        var uri = new StringBuilder(ApiRoutes.Basket.DecreaseProduct)
                        .Append($"?productId={productId}")
                        .ToString();
        
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(uri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnOk_WhenTheBasketHaveTheProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.First().Id.ToString();

        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={productId}")
                        .ToString();
        var decreaseProductUri = new StringBuilder(ApiRoutes.Basket.DecreaseProduct)
                        .Append($"?productId={productId}")
                        .ToString();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(decreaseProductUri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WhenUserHasProductsInBasket()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.First().Id.ToString();

        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={productId}")
                        .ToString();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Basket.GetProducts);

        string responseReadedAsString = await response.Content.ReadAsStringAsync();

        var basketProducts = JsonConvert.DeserializeObject<BasketResponse>(responseReadedAsString);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        basketProducts.Should().NotBeNull();

        basketProducts.Products.Count().Should().BeGreaterThanOrEqualTo(1);

        basketProducts.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnNotFound_WhenTheBasketDoesnHaveTheProduct()
    {
        // Arrange
        var invalidProductId = 0.ToString();

        var uri = new StringBuilder(ApiRoutes.Basket.IncreaseProduct)
                        .Append($"?productId={invalidProductId}")
                        .ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(uri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncreaseProduct_ShoulReturnOk_WhenBasketHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var validProductId = db.Products.Select(p => p.Id).First().ToString();

        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={validProductId}")
                        .ToString();
        var increaseProductUri = new StringBuilder(ApiRoutes.Basket.IncreaseProduct)
                        .Append($"?productId={validProductId}")
                        .ToString();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(increaseProductUri, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnNotFound_WhenTheUserDoesntHaveTheSpecificProductInBasket()
    {
        // Arrange
        string unExistingProductId = 100_000_000.ToString();

        var uri = new StringBuilder(ApiRoutes.Basket.RemoveProduct)
                        .Append($"?productId={unExistingProductId}")
                        .ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(uri);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnOk_WhenTheUserHaveTheSpecificProductInBasket()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.First().Id.ToString();

        var removeProductUri = new StringBuilder(ApiRoutes.Basket.RemoveProduct)
                        .Append($"?productId={productId}")
                        .ToString();
        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={productId}")
                        .ToString();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(removeProductUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductIds_ShouldReturnOk_WhenTheAreProductInCart()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.First().Id.ToString();

        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={productId}")
                        .ToString();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(ApiRoutes.Basket.GetProductIds);

        string responseReadedAsString = await response.Content.ReadAsStringAsync();

        var productIds = JsonConvert.DeserializeObject<int[]>(responseReadedAsString);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productIds.Length.Should().BeGreaterThanOrEqualTo(1);
    }
}