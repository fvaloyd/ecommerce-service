using Ecommerce.Application.Baskets;
using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;


namespace Ecommerce.Application.UnitTests.Baskets;

public class BasketServiceTest : IClassFixture<DbContextFixture>
{
    readonly IEcommerceDbContext _db;

    readonly Mock<IStoreService> storeServiceMock = new();

    public BasketServiceTest(DbContextFixture dbContextFixture)
    {
        _db = dbContextFixture.GetDbContext();
    }

    [Fact]
    public void BasketService_ShouldImplementIBasketService()
    {
        typeof(BasketService).Should().BeAssignableTo<IBasketService>();
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_ShouldReturnFalse_WhenTheStoreDoesntHaveABasketproductAssociated()
    {
        // Arrange
        var basket = new Basket(100_100, "", 10);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.RestoreTheQuantityIntoStore(basket);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_ShouldReturnTrue_WhenTheStoreHaveTheBasketproductAssociated()
    {
        // Arrange
        var basket = new Basket(1, "", 1);
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.RestoreTheQuantityIntoStore(basket);

        // Arrange
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnFalse_WhenNoProductsInStock()
    {
        // Arrange
        int productWithNoStockId = TestData.ProductStores.First(ps => ps.Quantity == 0).ProductId;

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.AddProductAsync(productWithNoStockId, "1");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnFalse_WhenTheBasketAlreadyHaveTheProduct()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ApplicationUserId == "1");

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.AddProductAsync(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnFalse_WhenErrorOcurrsWhileDecrementingTheStoreProduct()
    {
        // Arrange
        var productId = TestData.ProductStores.First(ps => ps.Quantity > 0).ProductId;

        var userId = "test";

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.AddProductAsync(productId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnTrue_WhenDecreaseTheProductFromTheStoreIsSuccessful()
    {
        // Arrange
        var productId = TestData.ProductStores.First(ps => ps.Quantity > 0).ProductId;

        var userId = "test";

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.AddProductAsync(productId, userId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnFalse_WhenTheUserBasketDoesNotContainTheProduct()
    {
        // Arrange
        var productId = 1;

        var userId = "test";

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.IncreaseProduct(productId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnFalse_WhenErrorOccursWhileDrecreaseTheProductFromStore()
    {
        // Arrange
        var basket = TestData.Baskets.Last();

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.IncreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var basket = TestData.Baskets.FirstOrDefault(b => b.ApplicationUserId == "1");

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.IncreaseProduct(basket!.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnFalse_WhenTheBasketDoesnHaveTheCurrentProduct()
    {
        // Arrange
        var userId = TestData.Baskets.First().ApplicationUserId;
        var productId = 100_000;

        var service = new BasketService(storeServiceMock.Object, _db);

        //Act
        var result = await service.DecreaseProduct(productId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnFalse_WhenIncreaseProductInStoreFail()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ProductId == 1);

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(false);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.DecreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnTrue_WhenDecreaseProductIsSuccessful()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ProductId == 1);

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.DecreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetAllProducts_ShouldThrowInvalidOperationException_WhenTheUserHasNoProductsInBasket()
    {
        // Arrange
        var userId = "test";

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var act = async () => await service.GetAllProducts(userId);

        // Assert
        act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAListOfProducts_WhenTheUserBasketContainsProducts()
    {
        // Arrange
        List<Basket> userBasket = TestData.Baskets.Where(b => b.ApplicationUserId == "1").ToList();

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        (IEnumerable<Product> result, float total) = await service.GetAllProducts(userBasket.First().ApplicationUserId);

        // Assert
        result.Count().Should().Be(userBasket.Count);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnFalse_WhenUserDoesNotHaveSpecificProductsInBasket()
    {
        // Arrange
        string userId = "1";

        int productId = 100_100_000;

        List<Basket> userBasket = TestData.Baskets.Where(b => b.Product == null).ToList();

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.RemoveProduct(productId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnTrue_WhenUserHasSpecificProductInBasket()
    {
        // Arrange
        Basket basket = TestData.Baskets.FirstOrDefault(b => b.ApplicationUserId == "1")!;

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.RemoveProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeTrue();
    }
}