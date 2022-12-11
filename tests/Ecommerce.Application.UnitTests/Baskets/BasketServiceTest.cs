using Ecommerce.Application.Baskets;
using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;


namespace Ecommerce.Application.UnitTests.Baskets;

public class BasketServiceTest
{
    readonly Mock<IStoreService> storeServiceMock = new();

    static Mock<IEcommerceDbContext> GetDbContextMock()
    {
        Mock<IEcommerceDbContext> dbContextMock = new();

        dbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        SetUpDbSets(dbContextMock);

        return dbContextMock;
    }

    static void SetUpDbSets(Mock<IEcommerceDbContext> mockDbContext)
    {
        var storeDbSet = TestData.Stores.AsQueryable().BuildMockDbSet();
        var productStoreDbSet = TestData.ProductStores.AsQueryable().BuildMockDbSet();
        var productDbSet = TestData.Products.AsQueryable().BuildMockDbSet();
        var brandDbSet = TestData.Brands.AsQueryable().BuildMockDbSet();
        var categoryDbSet = TestData.Categories.AsQueryable().BuildMockDbSet();
        var basketDbSet = TestData.Baskets.AsQueryable().BuildMockDbSet();


        mockDbContext.Setup(db => db.Baskets).Returns(basketDbSet.Object);
        mockDbContext.Setup(db => db.Stores).Returns(storeDbSet.Object);
        mockDbContext.Setup(db => db.ProductStores).Returns(productStoreDbSet.Object);
        mockDbContext.Setup(db => db.Products).Returns(productDbSet.Object);
        mockDbContext.Setup(db => db.Brands).Returns(brandDbSet.Object);
        mockDbContext.Setup(db => db.Categories).Returns(categoryDbSet.Object);
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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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
        
        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

        // Act
        var result = await service.IncreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var basket = TestData.Baskets.Last();

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

        // Act
        var result = await service.IncreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnFalse_WhenTheBasketDoesnHaveTheCurrentProduct()
    {
        // Arrange
        var userId = TestData.Baskets.First().ApplicationUserId;
        var productId = 100_000;

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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

        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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
        
        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

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
        
        var service = new BasketService(storeServiceMock.Object, GetDbContextMock().Object);

        // Act
        var result = await service.GetAllProducts(userBasket.First().ApplicationUserId);

        // Assert
        result.Count().Should().Be(userBasket.Count);
    }
}