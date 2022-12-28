using Ecommerce.Application.Data;
using Ecommerce.Application.Products;

namespace Ecommerce.Application.UnitTests.Stores;

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
    public async void DeleteProductStoreRelation_ShouldReturnFalse_WhenTheStoreDoesnHaveSpecificProduct()
    {
        // Arrange
        var service = new ProductService(_db);

        int productId = 100_000;

        // Act
        var result = await service.DeleteProductStoreRelation(productId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async void DeleteProductStoreRelation_ShouldReturnTrue_WhenTheStoreHaveTheSpecificProduct()
    {
        // Arrange
        var service = new ProductService(_db);

        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId == 1);

        // Act
        var result = await service.DeleteProductStoreRelation(productStore!.ProductId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RelatedToStoreAsync_ShoulReturnFalse_whenProductAlreadyRelatedToTheStore()
    {
        // Arrange
        var service = new ProductService(_db);

        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId == 1);

        // Act
        var result = await service.RelatedToStoreAsync(productStore!.ProductId, productStore.StoreId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RelateToStoreAsync_ShouldReturnTrue_WhenProductNoRelatedAlready()
    {
        // Arrange
        var service = new ProductService(_db);
        
        var storeId = TestData.Stores.First().Id;

        int productId = 100_000;

        // Act
        var result = await service.RelatedToStoreAsync(productId, storeId);

        // Assert
        result.Should().BeTrue();
    }
}