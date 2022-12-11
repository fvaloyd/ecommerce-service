using System.Reflection;
using Ecommerce.Application.Data;
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Identity;

public class EcommerceDbContext : IdentityDbContext<ApplicationUser>, IDbContext, IEcommerceDbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options){}

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<ProductStore> ProductStores => Set<ProductStore>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
