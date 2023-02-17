namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class IncreaseProductQuantityTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/basket/IncreaseProductQuantityInBasket?productId=";

    readonly string addProductEndPointPath = "api/basket/AddProductToBasket?productId=";

    public IncreaseProductQuantityTests(BaseIntegrationTest baseIntegrationTest)
    {
        this._baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenTheBasketDoesnHaveTheProduct()
    {
        // Arrange
        int invalidProductId = 0;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + invalidProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShoulReturnOk_WhenBasketHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = db.Products.Select(p => p.Id).First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductEndPointPath + validProductId, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}