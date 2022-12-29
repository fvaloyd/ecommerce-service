namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class AddProductTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/basket/addproduct?productId=";

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
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + invalidProductId.ToString(), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenValidProductIdIsSent()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync($"api/basket/removeproduct?productId={validProductId.ToString()}");
    
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId.ToString(), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenProductIsAlreadyInTheBasket()
    {
        // Arrange
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        int validProductId = Db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId.ToString(), null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId.ToString(), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}