using System.Linq.Expressions;
using Ecommerce.Application.Baskets;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;

namespace Ecommerce.Application.UnitTests.Baskets;
public class BasketServiceTest
{
    private readonly Store storeMock = new Store
    {
        Id = 1,
        Name = "moq store",
        State = true
    };
    private readonly ProductStore productStoreMock = new ProductStore();
    private readonly Basket basketMock = new();
    private readonly Product productMock = new();
    private readonly Mock<IEfRepository<Store>> storeRepoMoq = new Mock<IEfRepository<Store>>();
    private readonly Mock<IEfRepository<Product>> productRepoMoq = new Mock<IEfRepository<Product>>();
    private readonly Mock<IEfRepository<Basket>> basketRepoMoq = new Mock<IEfRepository<Basket>>();
    private readonly Mock<IEfRepository<ProductStore>> productStoreRepoMoq = new Mock<IEfRepository<ProductStore>>();
    private readonly Mock<IStoreService> storeServiceMoq = new Mock<IStoreService>();

    private BasketService CreateBasketServiceMock()
    {
        return new BasketService(
            storeRepoMoq.Object,
            basketRepoMoq.Object,
            productStoreRepoMoq.Object,
            storeServiceMoq.Object
        );
    }

    [Fact]
    public void Should_Implement_IBasketService()
    {
        typeof(BasketService).Should().BeAssignableTo<IBasketService>();
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_WithBasketWithNoProductStoreAssociate_ShouldReturnFalse()
    {
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(null!, null!)).Returns<ProductStore>(null);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.RestoreTheQuantityIntoStore(basketMock);

        result.Should().Be(false);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task RestoreTheQuantityIntoStore_WithBasketWithProductStoreAssociate_ShouldIncreaseTheProductStoreQuantityWithTheBasketQuantity(int basketQuantity)
    {
        basketMock.Quantity = basketQuantity;

        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);

        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);

        var basketServiceMock =  CreateBasketServiceMock();

        await basketServiceMock.RestoreTheQuantityIntoStore(basketMock);

        productStoreMock.Quantity.Should().Be(basketQuantity);
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_WhenSaveChangeDoesNotAffectAnyRow_ShouldReturnFalse()
    {
        basketMock.Quantity = 1;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);

        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        productStoreRepoMoq.Setup(ps => ps.SaveChangeAsync().Result).Returns(0);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.RestoreTheQuantityIntoStore(basketMock);

        result.Should().Be(false);
    }

    [Fact]
    public async Task RestoreTheQuantityIntoStore_WhenSaveChangeAffectAnyRow_ShouldReturnFalse()
    {
        basketMock.Quantity = 1;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);

        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);
        productStoreRepoMoq.Setup(ps => ps.SaveChangeAsync().Result).Returns(1);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.RestoreTheQuantityIntoStore(basketMock);

        result.Should().Be(true);
    }

    [Fact]
    public async Task AddProductAsync_WithNoProductInStock_ShouldReturnFalse()
    {
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns<ProductStore>(null);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.RestoreTheQuantityIntoStore(basketMock);

        result.Should().Be(false);
    }

    [Fact]
    public async Task AddProductAsync_WithSpecificProductInBasket_ShouldReturnFalse()
    {
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), It.IsAny<string>())).Returns(productStoreMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), null!)).Returns(basketMock);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.AddProductAsync(It.IsAny<int>(), It.IsAny<string>());

        result.Should().Be(false);
    }

    [Fact]
    public async Task AddProductAsync_FailWhenCreatingTheBasket_ShouldReturnFalse()
    {
        productStoreMock.Product = productMock;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), It.IsAny<string>())).Returns(productStoreMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), null!)).Returns<Basket>(null);
        basketRepoMoq.Setup(b => b.AddAsync(It.IsAny<Basket>()).Result).Returns<Basket>(null);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.AddProductAsync(It.IsAny<int>(), "");

        result.Should().Be(false);
    }

    [Fact]
    public async Task AddProductAsync_FailWhenDecreaseTheProductFromTheStore_ShouldReturnFalseAndRemoveTheBasket()
    {
        productStoreMock.Product = productMock;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), It.IsAny<string>())).Returns(productStoreMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), null!)).Returns<Basket>(null);
        basketRepoMoq.Setup(b => b.AddAsync(It.IsAny<Basket>()).Result).Returns(basketMock);
        storeServiceMoq.Setup(s => s.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(false);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.AddProductAsync(It.IsAny<int>(), "");

        result.Should().Be(false);
        basketRepoMoq.Verify(br => br.Remove(It.Is<Basket>(b => b.ProductId == basketMock.ProductId)), Times.Once);
    }

    [Fact]
    public async Task AddProductAsync_SuccessWhenDecreaseTheProductFromTheStore_ShouldReturnTrue()
    {
        productStoreMock.Product = productMock;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        productStoreRepoMoq.Setup(ps => ps.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), It.IsAny<string>())).Returns(productStoreMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), null!)).Returns<Basket>(null);
        basketRepoMoq.Setup(b => b.AddAsync(It.IsAny<Basket>()).Result).Returns(basketMock);
        storeServiceMoq.Setup(s => s.DecreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.AddProductAsync(It.IsAny<int>(), "");

        result.Should().Be(true);
    }

    [Fact]
    public async Task DecreaseProduct_WithNoProductAssociateInUserBasket_ShouldReturnFalse()
    {
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns<Basket>(null);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.DecreaseProduct(It.IsAny<int>(), It.IsAny<string>());

        result.Should().Be(false);
    }

    [Fact]
    public async Task DecreaseProduct_FailToIncreaseProductInStore_ShouldReturnFalse()
    {
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns(basketMock);
        storeServiceMoq.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(false);

        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.DecreaseProduct(It.IsAny<int>(), It.IsAny<string>());

        result.Should().Be(false);
    }

    [Fact]
    public async Task DecreaseProduct_SuccessIncreaseProductInStore_ShouldDecreaseProductQuantityFromBasketDecreaseTheTotal()
    {
        int basketQuantity = 2;
        int basketTotal = 200;
        float productPrice = 100;
        productMock.Price = productPrice;
        basketMock.Quantity = basketQuantity;
        basketMock.Total = basketTotal;
        basketMock.Product = productMock;
        Mock<Basket> userBasketMock = new();
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns(basketMock);
        storeServiceMoq.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);


        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.DecreaseProduct(It.IsAny<int>(), It.IsAny<string>());

        basketMock.Quantity.Should().Be(basketQuantity - 1);
        basketMock.Total.Should().Be(basketTotal - productPrice);
    }

    [Fact]
    public async Task DecreaseProduct_SuccessToPersisTheChanges_ShouldReturnTrue()
    {
        basketMock.Quantity = 2;
        basketMock.Total = 200;
        productMock.Price = 100;
        basketMock.Product = productMock;
        storeRepoMoq.Setup(s => s.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(b => b.GetFirst(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns(basketMock);
        storeServiceMoq.Setup(ss => ss.IncreaseProductAsync(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(true);
        basketRepoMoq.Setup(b => b.SaveChangeAsync().Result).Returns(1);


        var basketServiceMock =  CreateBasketServiceMock();

        var result = await basketServiceMock.DecreaseProduct(It.IsAny<int>(), It.IsAny<string>());

        result.Should().Be(true);
    }

    [Fact]
    public void GetAllProducts_WithNoUserBasket_ShouldThrowInvalidOperationException()
    {
        storeRepoMoq.Setup(sr => sr.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(br => br.GetAll(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns<IEnumerable<Basket>>(null);
        
        var basketServiceMock =  CreateBasketServiceMock();

        var act = () => basketServiceMock.GetAllProducts("");

        act.Should().Throw<InvalidOperationException>().WithMessage("The user did not have a basket associated");
    }

    [Fact]
    public void GetAllProducts_WithBasketAssociateAndProductOnBasket_Should_ReturnAListOfProducts()
    {
        basketMock.Product = productMock;

        IEnumerable<Basket> userBasketsMock = new List<Basket>()
        {
            basketMock,
        };

        storeRepoMoq.Setup(sr => sr.GetFirst(null!, null!)).Returns(storeMock);
        basketRepoMoq.Setup(br => br.GetAll(It.IsAny<Expression<Func<Basket, bool>>>(), It.IsAny<string>())).Returns(userBasketsMock);

        
        var basketServiceMock =  CreateBasketServiceMock();      

        var result = basketServiceMock.GetAllProducts("");

        result.Should().BeOfType<List<Product>>();
   }
}