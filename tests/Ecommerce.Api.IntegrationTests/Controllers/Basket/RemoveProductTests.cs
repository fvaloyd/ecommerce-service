namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class RemoveProductTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/basket/removeproduct?productId=";
    string addProductPath = "api/basket/addproduct?productId="; 

    public RemoveProductTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task RemoveProductWithoutHavingItInBasket_ShouldReturnBadRequest()
    {
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(endPointPath + "0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveProductWithProductInBasket_ShouldReturnNoContent()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var product = db.Products.First();
        var _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(addProductPath + product.Id.ToString(), null);
        var response = await _baseIntegrationTest.DefaultUserHttpClient.DeleteAsync(endPointPath + product.Id.ToString());

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}