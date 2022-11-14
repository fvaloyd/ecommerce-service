namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class DecreaseProductQuantityTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/basket/decreaseproductquantity?productId=";
    string addProductPath = "api/basket/addproduct?productId="; 

    public DecreaseProductQuantityTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }
    
    [Fact]
    public async Task DecreaseProductWithoutHavingItInBasket_ShouldReturnBadRequest()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var validProductId = db.Products.Select(p => p.Id).OrderByDescending(l => l).First().ToString();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId, null!);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WithProductInbasket_ShouldReturnOk()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var product = db.Products.First();
        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductPath + product.Id.ToString(), null);

        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + product.Id.ToString(), null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}