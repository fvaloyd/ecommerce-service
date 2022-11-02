using System.Linq.Expressions;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ecommerce.Infrastructure.UnitTests.Repository;

public class EfRepositoryTest
{
    private static Brand brand = new("test", true){Id = 1};
    private static Brand brand2 = new("test", true){Id = 2};
    private IEnumerable<Brand> brandList = new List<Brand>() { brand, brand2 };
    private static Product productWithLowPrice = new("test", 90.00f, 1, 1, "https://test.com"){Id = 1};
    private static Product product3 = new("test", 150.00f, 1, 1, "https://test.com"){Id = 3};
    private static Product product2 = new("test", 150.00f, 1, 1, "https://test.com"){Id = 2};
    private IEnumerable<Product> productList = new List<Product>() { productWithLowPrice, product2, product3 };
    private Mock<IDbContext> dbContextMock = new();
    private Mock<DbSet<Product>> productDbSetMock;
    private Mock<DbSet<Brand>> brandDbSetMock;
    public EfRepositoryTest()
    {
        productDbSetMock = MockDbSet(productList);
        productDbSetMock.Setup(p => p.FindAsync(1).Result).Returns(productWithLowPrice);
        brandDbSetMock = MockDbSet(brandList);
        dbContextMock.Setup(db => db.Products).Returns(productDbSetMock.Object);
        dbContextMock.Setup(db => db.Set<Product>()).Returns(productDbSetMock.Object);
        dbContextMock.Setup(db => db.Brands).Returns(brandDbSetMock.Object);
        dbContextMock.Setup(db => db.Set<Brand>()).Returns(brandDbSetMock.Object);
    }

    private Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> list) where T: class, new()
    {
        IQueryable<T> queryableList = list.AsQueryable();
        Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();
        dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
        return dbSetMock;
    }

    [Fact]
    public void ShouldImplementIEfRepository()
    {
        typeof(EfRepository<>).Should().BeAssignableTo(typeof(IEfRepository<>));
    }

    [Fact]
    public void ShouldCallDbSet()
    {
        _ = new EfRepository<Product>(dbContextMock.Object);

        dbContextMock.Verify(db => db.Set<Product>(), Times.Once);
    }

    [Fact]
    public void GetAllAsync_ShouldReturnAListOfProduct()
    {
        var repo  = new Mock<EfRepository<Product>>(dbContextMock.Object);
        var result = repo.Object.GetAll(null!, null!);

        result.Count().Should().Be(3);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAndReturnEntityAdded()
    {
        Expression<Func<Product, bool>> filter = p => p.Price > 100.00f;
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  
        var result = await repo.Object.AddAsync(product2);

        productDbSetMock.Verify(dbs => dbs.AddAsync(It.Is<Product>(p => p.Id == product2.Id), default), Times.Once);

        dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public void GetFirst_ShouldReturnASingleProduct()
    {
        int id = 1;
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  
        var result = repo.Object.GetFirst(p => p.Id == id, null!);

        result.Should().BeOfType<Product>();
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddAListOfEntitiesAndReturnTrue()
    {
        dbContextMock.Setup(db => db.SaveChangesAsync(default).Result).Returns(productList.Count());
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  

        var result = await repo.Object.AddRangeAsync(productList);

        productDbSetMock.Verify(dbs => dbs.AddRangeAsync(It.Is<IEnumerable<Product>>(p => p.Count() == productList.Count()), default), Times.Once);
        dbContextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveAEntity()
    {
        int id = 1;
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  

        await repo.Object.RemoveAsync(id);

        productDbSetMock.Verify(dbs => dbs.Remove(It.Is<Product>(p => p.Id == id)), Times.Once);
    }

    [Fact]
    public void Remove_ShouldRemoveAEntity()
    {
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  

        repo.Object.Remove(product2);

        productDbSetMock.Verify(dbs => dbs.Remove(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public void RemoveRange_ShouldRemoveAListOfEntities()
    {
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  

        repo.Object.RemoveRange(productList);

        productDbSetMock.Verify(dbs => dbs.AttachRange(It.IsAny<IEnumerable<Product>>()), Times.Once);
        productDbSetMock.Verify(dbs => dbs.RemoveRange(It.IsAny<IEnumerable<Product>>()), Times.Once);
    }

    [Fact]
    public void UpdateAsync_ShouldUpdateAEntity()
    {
        var repo = new Mock<EfRepository<Product>>(dbContextMock.Object);  

        repo.Object.Update(product2);

        productDbSetMock.Verify(dbs => dbs.Update(It.Is<Product>(p => p.Id == product2.Id)), Times.Once);
    }
}