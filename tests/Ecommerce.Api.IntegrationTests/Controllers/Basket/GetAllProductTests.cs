using Ecommerce.Api.Dtos.Basket;

namespace Ecommerce.Api.IntegrationTests.Controllers.Basket;

[Collection("BaseIntegrationTestCollection")]
public class GetAllProductTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string endPointPath = "api/basket/getallproduct";
    public GetAllProductTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task GetAllProductWithOutProductInBasket_ShouldReturnBadRequest()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var user = db.Users.FirstOrDefault(u => u.Email == "default@gmail.com");
        var userBasket = db.Baskets.Where(b => b.ApplicationUserId == user!.Id).ToList();
        db.Baskets.RemoveRange(userBasket);
        await db.SaveChangesAsync();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllProductWithProductInBasket_ShouldReturnOkWithAListOfProducts()
    {
        using var db = _baseIntegrationTest.EcommerceProgram.CreateApplicationDbContext();
        var product = db.Products.First();

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={product.Id}", null);
        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={product.Id + 1}", null);

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath);
        string responseReadedAsString = await response.Content.ReadAsStringAsync();
        BasketProductDto basketProducts = JsonConvert.DeserializeObject<BasketProductDto>(responseReadedAsString);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketProducts.Should().NotBeNull();
        basketProducts.Products.Count().Should().BeGreaterThanOrEqualTo(2);
        basketProducts.Total.Should().BeGreaterThan(0);
    }
}