using System.Linq.Expressions;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Services;

namespace Ecommerce.Core.UnitTests.Services;

public class StoreServiceTest
{
    private readonly ProductStore productStoreMock = new(1, 1, 2);
    private readonly Mock<IEfRepository<ProductStore>> productStoreRepoMock = new();

    private StoreService CreateStoreService()
    {
        return new StoreService(productStoreRepoMock.Object);
    }

    [Fact]
    public void Should_Implement_IStoreService()
    {
        typeof(StoreService).Should().BeAssignableTo<IStoreService>();
    }

    [Fact]
    public async Task AddProductAsync_WhenTheStoreAlreadyHaveTheProduct_ShouldReturnFalse()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        var storeServiceMock = CreateStoreService();            

        var result = await storeServiceMock.AddProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(false);
    }

    [Fact]
    public async Task AddProductAsync_WhenTheStoreDoesntHaveTheProduct_AddTheProduct()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns<ProductStore>(null);
        productStoreRepoMock.Setup(psr => psr.AddAsync(It.IsAny<ProductStore>()).Result).Returns(productStoreMock);
        var storeServiceMock = CreateStoreService();

        var result = await storeServiceMock.AddProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(true);
        productStoreRepoMock.Verify(ps => ps.AddAsync(It.IsAny<ProductStore>()), Times.Once);
    }

    [Fact]
    public async Task DecreaseProductAsync_WhenStoreDoesntHaveSpecificProduct_ShouldReturnFalse()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns<ProductStore>(null);
        var storeServiceMock = CreateStoreService();            

        var result = await storeServiceMock.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(false);
    }

    public async Task DecreaseProductAsync_WhenProductQuantityAreEqualToOne_ShouldRemoveTheProductFromTheStore()
    {
        productStoreMock.Quantity = 1;
        int removeProductCall = 0;
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        productStoreRepoMock.Setup(psr => psr.Remove(It.IsAny<ProductStore>())).Callback(() => ++removeProductCall);
        productStoreRepoMock.Setup(psr => psr.SaveChangeAsync().Result).Returns(1);
        var storeServiceMock = CreateStoreService();

        var result = await storeServiceMock.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(true);
        removeProductCall.Should().Be(1);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(2)]
    public async Task DecreaseProductAsync_WhenProductQuantityAreGreaterToOne_ShouldDecreaseQuantityPersistChangeAndReturnTrue(int quantity)
    {
        productStoreMock.Quantity = quantity;
        int defaultAmountToDecrease = 1;
        int saveChangeCall = 0;
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        productStoreRepoMock.Setup(psr => psr.SaveChangeAsync().Result).Returns(1).Callback(() => ++saveChangeCall);
        var storeServiceMock = CreateStoreService();

        var result = await storeServiceMock.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(true);
        saveChangeCall.Should().Be(1);
        productStoreMock.Quantity.Should().Be(quantity - defaultAmountToDecrease);
    }

    [Fact]
    public async Task DeleteRelationProduct_WithUnExistProductStoreRelation_ShouldThrowInvalidOperationException()
    {
        productStoreRepoMock.Setup(psr => psr.GetAllAsync(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!).Result).Returns<IEnumerable<ProductStore>>(null);

        var storeServiceMock = CreateStoreService();

        Func<Task> act = () => storeServiceMock.DeleteProductStoreRelation(It.IsAny<int>());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Store doesn't exist.");
    }

    [Fact]
    public async Task DeleteRelationProduct_WhenExistAListOfProductStore_ShouldRemoveTheListOfProductStores()
    {
        IEnumerable<ProductStore> productStoresMock = new List<ProductStore>
        {
            productStoreMock
        };
        int removeRangeProductStoresCall = 0;
        productStoreRepoMock.Setup(psr => psr.GetAllAsync(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!).Result).Returns(productStoresMock);
        productStoreRepoMock.Setup(psr => psr.RemoveRange(It.IsAny<IEnumerable<ProductStore>>())).Callback(() => ++removeRangeProductStoresCall);

        var storeServiceMock = CreateStoreService();

        await storeServiceMock.DeleteProductStoreRelation(It.IsAny<int>());

        removeRangeProductStoresCall.Should().Be(1);
    } 

    [Fact]
    public async Task IncreaseProductAsync_WhenStoreDoesntHaveSpecificProduct_ShouldReturnFalse()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns<ProductStore>(null);
        var storeServiceMock = CreateStoreService();            

        var result = await storeServiceMock.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(false);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(22)]
    public async Task IncreaseProductAsync_WhenStoreHaveTheSpecificProduct_ShouldIncreaseQuantityPersistChangeAndReturnTrue(int productStoreQuantity)
    {
        productStoreMock.Quantity = productStoreQuantity;
        int saveChangeCall = 0;
        int defaultAmountToIncrease = 1;
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        productStoreRepoMock.Setup(psr => psr.SaveChangeAsync().Result).Returns(1).Callback(() => ++saveChangeCall);
        var storeServiceMock = CreateStoreService();

        var result = await storeServiceMock.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>());

        result.Should().Be(true);
        saveChangeCall.Should().Be(1);
        productStoreMock.Quantity.Should().Be(productStoreQuantity + defaultAmountToIncrease);
    }
}