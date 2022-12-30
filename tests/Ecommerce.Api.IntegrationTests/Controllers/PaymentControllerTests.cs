using Ecommerce.Infrastructure.Payment.Models;

namespace Ecommerce.Api.IntegrationTests.Controllers;
[Collection("BaseIntegrationTestCollection")]
public class PaymentControllerTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "api/payment/";

    public PaymentControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task Pay_ShouldReturnBadRequest_WhenTheUserDoesnHaveProductInBasket()
    {
        // Arrange
        var card = new PayRequest("3434343434343434", "11", "2023", "314");

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(endPointPath + "pay", card);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Pay_ShouldReturnOk_WhenTheUserHaveProductInBasket()
    {
        // Arrange
        var card = new PayRequest("3434343434343434", "11", "2023", "314");

        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();

        var productId = db.Products.Select(p => p.Id).First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={productId}", null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(endPointPath + "pay", card);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
