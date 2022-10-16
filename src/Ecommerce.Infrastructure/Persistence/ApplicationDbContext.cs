using System.Reflection;
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Identity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<ProductStore> ProductStores { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Basket> Baskets { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
