namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class RemoveProductTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/basket/removeproduct?productId=";

    readonly string addProductPath = "api/basket/addproduct?productId="; 

    public RemoveProductTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnNotFound_WhenTheUserDoesntHaveTheSpecificProductInBasket()
    {
        // Arrange
        int unExistingProductId = 100_000_000;

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(endPointPath + unExistingProductId.ToString());
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnNoContent_WhenTheUserHaveTheSpecificProductInBasket()
    {
        // Arrange
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var product = db.Products.First();

        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductPath + product.Id.ToString(), null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(endPointPath + product.Id.ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}