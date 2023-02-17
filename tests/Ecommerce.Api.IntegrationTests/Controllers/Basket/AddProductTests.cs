namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class AddProductTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/basket/AddProductToBasket?productId=";
    const string DeleteProductFromBasketPath = "api/basket/RemoveProductFromBasket?productId=";

    public AddProductTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvalidProductIdIsSent()
    {
        // Arrange
        var invalidProductId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + invalidProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenValidProductIdIsSent()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(DeleteProductFromBasketPath + validProductId);
    
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenProductIsAlreadyInTheBasket()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}