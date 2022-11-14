namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class AddProductTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/basket/addproduct?productId=";

    public AddProductTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task WithInvalidProductId_ShouldReturnBadRequest()
    {
        var invalidProductId = 0;
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + invalidProductId.ToString(), null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WithValidProductId_ShouldReturnOk()
    {
        using var Db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        int validProductId = Db.Products.Select(p => p.Id).First();

        var addProduct = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(endPointPath + validProductId.ToString(), null);
        var addproductReaded = await addProduct.Content.ReadAsStringAsync();

        addProduct.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}