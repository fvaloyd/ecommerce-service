using Ecommerce.Application.Data;

namespace Ecommerce.Application.UnitTests;
public class DbContextFixture : IDisposable
{
    public IEcommerceDbContext GetDbContext()
    {
        Mock<IEcommerceDbContext> dbContextMock = new();

        dbContextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        SetUpDbSets(dbContextMock);

        return dbContextMock.Object;
    }

    private static void SetUpDbSets(Mock<IEcommerceDbContext> mockDbContext)
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

    public void Dispose()
    {
    }
}
