namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class DecreaseProductQuantityTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/basket/decreaseproductquantity?productId=";

    readonly string addProductPath = "api/basket/addproduct?productId="; 

    public DecreaseProductQuantityTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_WhenTheBasketDoesnHaveTheSpecificProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).OrderByDescending(l => l).First().ToString();

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + productId, null!);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenTheBasketHaveTheProduct()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var product = db.Products.First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductPath + product.Id.ToString(), null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + product.Id.ToString(), null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}