using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Persistence.Identity;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task Handle(EcommerceDbContext _context, ILogger _logger)
    {
        try
        {
            _logger.LogInformation("Seeding data...");
            if (_context.Database.IsSqlServer())
            {
                _context.Database.Migrate();
            }
            if (!await _context.Stores.AnyAsync())
            {
                await _context.Stores.AddAsync(GetStore());
                await _context.SaveChangesAsync();
            }
            if (!await _context.Categories.AnyAsync())
            {
                await _context.Categories.AddRangeAsync(GetCategories());
                await _context.SaveChangesAsync();
            }
            if (!await _context.Brands.AnyAsync())
            {
                await _context.Brands.AddRangeAsync(GetBrands());
                await _context.SaveChangesAsync();
            }
            if (!await _context.Products.AnyAsync())
            {
                await _context.Products.AddRangeAsync(GetProducts());
                await _context.SaveChangesAsync();
            }
            if (!await _context.ProductStores.AnyAsync())
            {
                await _context.ProductStores.AddRangeAsync(GetProductStores());
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error seeding");
        }
    }
    static Store GetStore()
    {
        return new Store("Default");
    }
    static IEnumerable<ProductStore> GetProductStores()
    {
        return new List<ProductStore>
        {
            new ProductStore{ProductId = 1, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 2, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 3, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 4, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 5, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 6, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 7, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 8, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 9, StoreId = 1, Quantity = 100},
            new ProductStore{ProductId = 10, StoreId = 1, Quantity = 100},
        };
    }
    static IEnumerable<Brand> GetBrands()
    {
        return new List<Brand>
        {
            new Brand("Hp",         true),
            new Brand("Dell",       true),
            new Brand("Xiaomi",     true),
            new Brand("Apple",      true),
            new Brand("Keycron",    true),
            new Brand("Razer",      true),
            new Brand("Logitec",   true),
            new Brand("Skullcandy", true),
        };
    }
    static IEnumerable<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category("Computer",   true),
            new Category("Cell Phone", true),
            new Category("Keyboard",   true),
            new Category("Mouse",      true),
            new Category("Headphones", true),
        };
    }
    static List<Product> GetProducts()
    {
        return new List<Product>
        {
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1665071963/Ecommerce/Xiaomi-11t-Pro.jpg")              {Name = "Xiaomi 11t Pro",   Price = 400f,  CategoryId = 2, BrandId = 3},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1665072046/Ecommerce/Hp-Pavilion-15.jpg")              {Name = "Hp Pavilion 15",   Price = 600f,  CategoryId = 1, BrandId = 1},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1665072015/Ecommerce/Dell-Inspiron-15.jpg")            {Name = "Dell Inspiron 15", Price = 800f,  CategoryId = 1, BrandId = 2},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1681130440/Ecommerce/iphone-10.jpg")                   {Name = "Iphone X",         Price = 1000f, CategoryId = 2, BrandId = 4},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1677643040/Ecommerce/Logitec-mouse-g502.jpg")          {Name = "Logitec g502",     Price = 1000f, CategoryId = 4, BrandId = 7},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1677643269/Ecommerce/Skullcandy-headphones-coral.jpg") {Name = "Skullcandy Coral", Price = 100f,  CategoryId = 5, BrandId = 8},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1677642887/Ecommerce/Logitec.jpg")                     {Name = "Logitec keyborad", Price = 250f,  CategoryId = 3, BrandId = 7},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1681130931/Ecommerce/airpods-pro-3.jpg")               {Name = "AirPods Pro 3",    Price = 100f,  CategoryId = 5, BrandId = 4},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1681130970/Ecommerce/razerBlade.jpg")                  {Name = "Razer Blade 15",   Price = 2500f, CategoryId = 1, BrandId = 6},
            new Product("https://res.cloudinary.com/drxp8iwrd/image/upload/v1681130989/Ecommerce/keychron.jpg")                    {Name = "Custom Keychron",  Price = 300f,  CategoryId = 3, BrandId = 5},
        };
    }
}
