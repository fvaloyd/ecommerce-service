using Ecommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Api.IntegrationTests.Controllers;
[Collection("BaseIntegrationTestCollection")]
public class PaymentControllerTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/payment/";
    public PaymentControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task Pay_WithNoProductInBasket_ShouldReturnBadRequest()
    {
        var card = new CardOptions()
        {
            Cvc = "314",
            ExpMonth = "11",
            ExpYear = "2023",
            Number = "3434343434343434"
        };

        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(endPointPath + "pay", card);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Pay_WithProductInBasket_ShouldReturnOk()
    {
        var card = new CardOptions()
        {
            Cvc = "314",
            ExpMonth = "11",
            ExpYear = "2023",
            Number = "3434343434343434"
        };
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var productId = db.Products.Select(p => p.Id).First();
        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={productId}", null);

        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(endPointPath + "pay", card);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
