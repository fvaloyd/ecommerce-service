using Ecommerce.Application.Data;
using Ecommerce.Application.Products;
using Francisvac.Result;

namespace Ecommerce.Application.UnitTests.Products;

public class ProductServiceTest : IClassFixture<DbContextFixture>
{
    private readonly IEcommerceDbContext _db;

    public ProductServiceTest(DbContextFixture dbContextMockFixture)
    {
        _db = dbContextMockFixture.GetDbContext();
    }

    [Fact]
    public void ShouldImplementIProductService()
    {
        typeof(ProductService).Should().BeAssignableTo<IProductService>();
    }

    [Fact]
    public async void DeleteProductStoreRelation_ShouldReturnNotFoundResult_WhenTheStoreDoesnHaveSpecificProduct()
    {
        // Arrange
        var service = new ProductService(_db);

        int productId = 100_000;

        // Act
        Result result = await service.DeleteProductStoreRelation(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async void DeleteProductStoreRelation_ShouldReturnSuccessResult_WhenTheStoreHaveTheSpecificProduct()
    {
        // Arrange
        var service = new ProductService(_db);

        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId == 1);

        // Act
        Result result = await service.DeleteProductStoreRelation(productStore!.ProductId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }

    [Fact]
    public async Task RelatedToStoreAsync_ShoulReturnErrorResult_whenProductAlreadyRelatedToTheStore()
    {
        // Arrange
        var service = new ProductService(_db);

        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId == 1);

        // Act
        Result result = await service.RelatedToStoreAsync(productStore!.ProductId, productStore.StoreId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.Error);
    }

    [Fact]
    public async Task RelateToStoreAsync_ShouldReturnSuccessResult_WhenProductNoRelatedAlready()
    {
        // Arrange
        var service = new ProductService(_db);

        var storeId = TestData.Stores.First().Id;

        int productId = 100_000;

        // Act
        Result result = await service.RelatedToStoreAsync(productId, storeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Response.ResultStatus.Should().Be(ResultStatus.Success);
    }
}