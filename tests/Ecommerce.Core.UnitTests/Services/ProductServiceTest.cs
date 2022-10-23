using System.Linq.Expressions;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Services;

namespace Ecommerce.Core.UnitTests.Services;

public class ProductServiceTest
{
    private readonly Product productMock = new("product", 200.00f, 1, 1, "https://url.com"){Id = 1};
    private readonly Store storeMock = new("store"){Id = 1};
    private readonly ProductStore productStoreMock = new(1, 1, 1);
    private readonly Mock<IEfRepository<Product>> productRepoMock = new();
    private readonly Mock<IEfRepository<Store>> storeRepoMock = new();
    private readonly Mock<IEfRepository<ProductStore>> productStoreRepoMock = new();

    private ProductService CreateProductService()
    {
        return new ProductService(
            productStoreRepoMock.Object
        );
    }

    [Fact]
    public void Should_Implement_IProductService()
    {
        typeof(ProductService).Should().BeAssignableTo<IProductService>();
    }

    [Fact]
    public async Task DeleteProductStoreRelation_WithNoSpecificProductInStore_ShouldThrowInvalidOperationException()
    {
        productStoreRepoMock.Setup(psr => psr.GetAllAsync(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!).Result).Returns<ProductStore>(null);

        var productServiceMock = CreateProductService();

        Func<Task> act = () => productServiceMock.DeleteProductStoreRelation(productMock.Id);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Store doesnt have a product with Id: ${productMock.Id}");
    }

    [Fact]
    public async Task DeleteProductStoreRelation_WithSpecificProductInStore_ShouldRemoveTheStoreProductRelatedWithSpecificProduct()
    {
        IEnumerable<ProductStore> productStores = new List<ProductStore>()
        {
            productStoreMock
        };

        int removeStoreCall = 0;

        productStoreRepoMock.Setup(psr => psr.GetAllAsync(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!).Result).Returns(productStores);
        productStoreRepoMock.Setup(psr => psr.RemoveRange(It.IsAny<IEnumerable<ProductStore>>())).Callback(() => ++removeStoreCall);

        var productServiceMock = CreateProductService();

        await productServiceMock.DeleteProductStoreRelation(productMock.Id);

        removeStoreCall.Should().Be(1);
    }

    [Fact]
    public async Task RelatedToStoreAsync_WithProductAlreadyRelatedToTheStore_ShouldThrowInvalidOperationException()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns(productStoreMock);

        var productServiceMock = CreateProductService();

        Func<Task> act = () => productServiceMock.RelatedToStoreAsync(It.IsAny<int>(), It.IsAny<int>());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("The product is already related to the store.");
    }

    [Fact]
    public async Task RelateToStoreAsync_WithProductNoRelatedAlready_ShouldCreateTheRelation()
    {
        productStoreRepoMock.Setup(psr => psr.GetFirst(It.IsAny<Expression<Func<ProductStore, bool>>>(), null!)).Returns<ProductStore>(null);       
        productStoreRepoMock.Setup(psr => psr.AddAsync(It.IsAny<ProductStore>()).Result).Returns(productStoreMock);

        var productServiceMock = CreateProductService();

        var result = await productServiceMock.RelatedToStoreAsync(productMock.Id, storeMock.Id);

        result.Should().Be(1);
    }
}