using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Data;

public interface IEcommerceDbContext
{
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