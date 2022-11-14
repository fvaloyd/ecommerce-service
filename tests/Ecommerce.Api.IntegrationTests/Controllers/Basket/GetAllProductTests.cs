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
        // var user = db.Users.FirstOrDefault(u => u.Email == "default@gmail.com");
        // var basketToClean = db.Baskets.Where(b => b.ApplicationUserId == user!.Id).ToList();
        var product = db.Products.First();
        // var product2 = db.Products.Where(p => p.Id == product1.Id + 1).First();
        // db.Baskets.RemoveRange(basketToClean);

        // List<Ecommerce.Core.Entities.Basket> userBasket = new()
        // {
        //     new Ecommerce.Core.Entities.Basket(product1, user!.Id),
        //     new Ecommerce.Core.Entities.Basket(product2, user!.Id)
        // };

        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={product.Id}", null);
        _ = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync($"api/basket/addproduct?productId={product.Id + 1}", null);

        // db.Baskets.AddRange(userBasket);
        // await db.SaveChangesAsync();

        var response = await _baseIntegrationTest.DefaultUserHttpClient.GetAsync(endPointPath);
        string responseReadedAsString = await response.Content.ReadAsStringAsync();
        BasketProductDto basketProducts = JsonConvert.DeserializeObject<BasketProductDto>(responseReadedAsString);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        basketProducts.Should().NotBeNull();
        basketProducts.Products.Count().Should().BeGreaterThanOrEqualTo(2);
        basketProducts.Total.Should().BeGreaterThan(0);
    }
}