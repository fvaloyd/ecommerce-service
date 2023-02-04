using Ecommerce.Application.Baskets;
using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;
using Francisvac.Result;

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
    public async Task RestoreTheQuantityIntoStore_ShouldReturnNotFoundResult_WhenTheStoreDoesntHaveABasketproductAssociated()
    {
        // Arrange
        var basket = new Basket(100_100, "", 10);

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.RestoreTheQuantityIntoStore(basket);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_ShouldReturnSuccessResult_WhenTheStoreHaveTheBasketproductAssociated()
    {
        // Arrange
        var basket = new Basket(1, "", 1);
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.RestoreTheQuantityIntoStore(basket);

        // Arrange
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnErrorResult_WhenNoProductsInStock()
    {
        // Arrange
        int productWithNoStockId = TestData.ProductStores.First(ps => ps.Quantity == 0).ProductId;

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.AddProductAsync(productWithNoStockId, "1");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnErrorResult_WhenTheBasketAlreadyHaveTheProduct()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ApplicationUserId == "1");

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.AddProductAsync(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnErrorResult_WhenErrorOcurrsWhileDecrementingTheStoreProduct()
    {
        // Arrange
        var productId = TestData.ProductStores.First(ps => ps.Quantity > 0).ProductId;

        var userId = "test";

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Result.Error(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.AddProductAsync(productId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnSuccessResult_WhenDecreaseTheProductFromTheStoreIsSuccessful()
    {
        // Arrange
        var productId = TestData.ProductStores.First(ps => ps.Quantity > 0).ProductId;

        var userId = "test";

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Result.Success(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        var result = await service.AddProductAsync(productId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnNotFoundResult_WhenTheUserBasketDoesNotContainTheProduct()
    {
        // Arrange
        var productId = 1;

        var userId = "test";

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.IncreaseProduct(productId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnErrorResult_WhenErrorOccursWhileDrecreaseTheProductFromStore()
    {
        // Arrange
        var basket = TestData.Baskets.Last();

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Result.Error(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.IncreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task IncreaseProduct_ShouldReturnSuccessResult_WhenSuccess()
    {
        // Arrange
        var basket = TestData.Baskets.FirstOrDefault(b => b.ApplicationUserId == "1");

        storeServiceMock.Setup(ss => ss.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(Result.Success(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.IncreaseProduct(basket!.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnNotFoundResult_WhenTheBasketDoesnHaveTheCurrentProduct()
    {
        // Arrange
        var userId = TestData.Baskets.First().ApplicationUserId;
        var productId = 100_000;

        var service = new BasketService(storeServiceMock.Object, _db);

        //Act
        Result result = await service.DecreaseProduct(productId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnErrorResult_WhenIncreaseProductInStoreFail()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ProductId == 1);

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(Result.Error(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.DecreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task DecreaseProduct_ShouldReturnSuccessResult_WhenDecreaseProductIsSuccessful()
    {
        // Arrange
        var basket = TestData.Baskets.First(b => b.ProductId == 1);

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(Result.Success(""));

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.DecreaseProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnNotFoundResult_WhenTheUserHasNoProductsInBasket()
    {
        // Arrange
        string userId = "test";

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(Result.Success(""));
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        //var act = async () => await service.GetAllProducts(userId);
        Result<(IEnumerable<Product>, float)> result = await service.GetAllProducts(userId);

        // Assert
        //act.Should().ThrowAsync<InvalidOperationException>();
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
        result.Data.Item1.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnAListOfProducts_WhenTheUserBasketContainsProducts()
    {
        // Arrange
        List<Basket> userBasket = TestData.Baskets.Where(b => b.ApplicationUserId == "1").ToList();

        storeServiceMock.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(Result.Success(""));
        
        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result<(IEnumerable<Product>, float)> result = await service.GetAllProducts(userBasket.First().ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
        result.Data.Item1.Should().NotBeNullOrEmpty();
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
        Result result = await service.RemoveProduct(productId, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task RemoveProduct_ShouldReturnTrue_WhenUserHasSpecificProductInBasket()
    {
        // Arrange
        Basket basket = TestData.Baskets.FirstOrDefault(b => b.ApplicationUserId == "1")!;

        var service = new BasketService(storeServiceMock.Object, _db);

        // Act
        Result result = await service.RemoveProduct(basket.ProductId, basket.ApplicationUserId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }
}