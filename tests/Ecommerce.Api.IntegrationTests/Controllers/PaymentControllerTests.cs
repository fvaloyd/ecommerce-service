using Ecommerce.Contracts;
using Ecommerce.Infrastructure.Payment.Models;

using System.Text;

namespace Ecommerce.Api.IntegrationTests.Controllers;
[Collection("BaseIntegrationTestCollection")]
public class PaymentControllerTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

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
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Payment.Pay, card);

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

        var addProductUri = new StringBuilder(ApiRoutes.Basket.AddProduct)
                        .Append($"?productId={productId}")
                        .ToString();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductUri, null);

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(ApiRoutes.Payment.Pay, card);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}