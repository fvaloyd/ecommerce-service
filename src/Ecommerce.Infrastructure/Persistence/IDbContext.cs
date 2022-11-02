using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public interface IDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DbSet<Product> Products { get; }
    DbSet<Store> Stores { get; }
    DbSet<ProductStore> ProductStores { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Category> Categories { get; }
    DbSet<Basket> Baskets { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderDetail> OrderDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}