using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.Persistence;

public class SeedData
{
    public static async Task Handle(ApplicationDbContext _context, ILogger _logger)
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
                int brandId = _context.Brands.Select(b => b.Id).FirstOrDefault();
                int categoryId = _context.Categories.Select(c => c.Id).FirstOrDefault();
                List<Product> products = GetProducts().ToList();
                products.ForEach(p => {
                    p.BrandId = brandId;
                    p.CategoryId = categoryId;
                });
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }
            if (!await _context.ProductStores.AnyAsync())
            {
                var storeId = _context.Stores.Select(s => s.Id).FirstOrDefault();
                var productStores = GetProductStores().ToList();
                var products = _context.Products.ToArray();
                int count = 0;

                while(count != productStores.Count())
                {
                    productStores[count].ProductId = products[count].Id;
                    productStores[count].StoreId = storeId;
                    count++;
                }
                await _context.ProductStores.AddRangeAsync(productStores);
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
        return new Store
        {
            Name = "Default",
            State = true
        };
    }
    static IEnumerable<ProductStore> GetProductStores()
    {
        return new List<ProductStore> 
        {
            new ProductStore
            {
                Quantity = 10
            },
            new ProductStore
            {
                Quantity = 10
            },
            new ProductStore
            {
                Quantity = 10
            }
        };
    }
    static IEnumerable<Brand> GetBrands()
    {
        return new List<Brand>
        {
            new Brand
            {
                Name = "Hp",
                State = true
            },
            new Brand
            {
                Name = "Dell",
                State = true
            },
            new Brand
            {
                Name = "Xiaomi",
                State = true
            }
        };
    }
    static IEnumerable<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category
            {
                Name = "Computer",
                State = true
            },
            new Category
            {
                Name = "Cell Phone",
                State = true
            }
        };
    }
    static IEnumerable<Product> GetProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Name = "Xiaomi 11t Pro",
                Price = 400,
                ImageUrl = "https://res.cloudinary.com/drxp8iwrd/image/upload/v1665071963/Ecommerce/Xiaomi-11t-Pro.jpg"
            },
            new Product
            {
                Name = "Hp Pavilion 15",
                Price = 600,
                ImageUrl = "https://res.cloudinary.com/drxp8iwrd/image/upload/v1665072046/Ecommerce/Hp-Pavilion-15.jpg"
            },
            new Product
            {
                Name = "Dell Inspiron 15",
                Price = 800,
                ImageUrl = "https://res.cloudinary.com/drxp8iwrd/image/upload/v1665072015/Ecommerce/Dell-Inspiron-15.jpg"
            }
        };
    }
}
