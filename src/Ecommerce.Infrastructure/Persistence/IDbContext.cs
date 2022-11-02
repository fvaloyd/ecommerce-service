using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public interface IDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<Store> Stores { get; set; }
    DbSet<ProductStore> ProductStores { get; set; }
    DbSet<Brand> Brands { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<Basket> Baskets { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderDetail> OrderDetails { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}