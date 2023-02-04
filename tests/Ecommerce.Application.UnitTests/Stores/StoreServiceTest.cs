using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;
using Francisvac.Result;

namespace Ecommerce.Application.UnitTests.Stores;

public class StoreServiceTest : IClassFixture<DbContextFixture>
{
    private readonly IEcommerceDbContext _db;

    public StoreServiceTest(DbContextFixture dbContextFixture)
    {
        _db = dbContextFixture.GetDbContext();
    }

    [Fact]
    public void StoreService_ShouldImplementIStoreService()
    {
        typeof(StoreService).Should().BeAssignableTo<IStoreService>();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnFalse_WhenTheStoreAlreadyHaveTheProduct()
    {
        // Arrange
        ProductStore productAlreadyInStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId == 1)!;

        var service = new StoreService(_db);

        // Act
        Result result = await service.AddProductAsync(productId: productAlreadyInStore.ProductId, storeId: productAlreadyInStore.StoreId);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AddProductAsync_ShouldReturnTrue_WhenTheStoreDoesntHaveTheProduct()
    {
        // Arrange
        int productId = 100_000;

        int storeId = TestData.Stores.First().Id;

        var service = new StoreService(_db);

        // Act
        Result result = await service.AddProductAsync(productId, storeId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DecreaseProductAsync_ShouldReturnFalse_WhenTheStoreDoesntHaveTheSpecificProduct()
    {
        // Arrange
        int productId = 100_000;

        int storeId = TestData.Stores.First().Id;

        var service = new StoreService(_db);

        // Act
        Result result = await service.DecreaseProductAsync(productId, storeId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task DecreaseProductAsync_ShouldRemoveTheProduct_WhenProductQuantityAreLessOrEqualThanOne()
    {
        // Arrange
        var productStoreWithZeroQuantity = TestData.ProductStores.FirstOrDefault(ps => ps.Quantity == 0);
        
        var service = new StoreService(_db);

        // Act
        Result result = await service.DecreaseProductAsync(productStoreWithZeroQuantity!.ProductId, productStoreWithZeroQuantity.StoreId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DecreaseProductAsync_ShouldReturnTrue_WhenProductQuantityAreGreaterThanOne()
    {
        // Arrange
        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.Quantity > 1);

        var service = new StoreService(_db);

        // Act
        Result result = await service.DecreaseProductAsync(productStore!.ProductId, productStore.StoreId);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async void DeleteProductStoreRelation_ShouldFalse_WhenNotFoundProductStoreRelation()
    {
        // Arrange
        int storeIdWithNoProductRelated = 100_000;
        var service = new StoreService(_db);

        // Act
        Result result = await service.DeleteProductStoreRelation(storeIdWithNoProductRelated);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async void DeleteProductStoreRelation_ShouldReturnTrue_WhenExistAProductStoreRelation()
    {
        // Arrange
        var storeWithRelatedProducts = TestData.Stores.FirstOrDefault(s => s.Id == 1);
        
        var service = new StoreService(_db);

        // Act
        Result result = await service.DeleteProductStoreRelation(storeWithRelatedProducts!.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
    } 

    [Fact]
    public async Task IncreaseProductAsync_ShouldReturnFalse_WhenStoreDoesntHaveSpecificProduct()
    {
        // Arrange
        int productId = 100_000;

        int storeId = 100_000;

        var service = new StoreService(_db);

        // Act
        Result result = await service.IncreaseProductAsync(productId, storeId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task IncreaseProductAsync_ShouldIncreaseQuantityPersistChangeAndReturnTrue_WhenStoreHaveTheSpecificProduct()
    {
        // Arrange
        var productStore = TestData.ProductStores.FirstOrDefault(ps => ps.StoreId== 1);
        
        var service = new StoreService(_db);

        // Act
        Result result = await service.IncreaseProductAsync(productStore!.ProductId, productStore.StoreId); 
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnFalse_WhenTheStoreDoesnExist()
    {
        // Arrange
        int unExistingStoreId = 100_000;

        var service = new StoreService(_db);

        // Act
        Result result = await service.DeleteStore(unExistingStoreId); 
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Response.ResultStatus.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task DeleteStore_ShouldReturnTrue_WhenValidStoreIdIsPassed()
    {
        // Arrange
        int storeId = TestData.Stores.FirstOrDefault(s => s.Id == 1)!.Id;

        var service = new StoreService(_db);

        // Act
        Result result = await service.DeleteStore(storeId); 
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}