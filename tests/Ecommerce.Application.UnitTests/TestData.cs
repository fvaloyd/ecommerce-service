using Ecommerce.Core.Entities;

namespace Ecommerce.Application.UnitTests;
static class TestData
{
    public static List<Store> Stores => new List<Store>
    {
        new Store
        {
            Id = 1,
            Name = "test",
            State = true
        },
        new Store
        {
            Id = 2,
            Name = "test2",
            State = true
        },
        new Store
        {
            Id = 3,
            Name = "test3",
            State = false
        },

    };

    public static List<ProductStore> ProductStores => new List<ProductStore>
    {
        new ProductStore
        {
            ProductId = 1,
            StoreId = 1,
            Store = Stores.FirstOrDefault(s => s.Id == 1)!,
            Product = Products.FirstOrDefault(p => p.Id == 1)!,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 2,
            StoreId = 1,
            Store = Stores.FirstOrDefault(s => s.Id == 1)!,
            Product = Products.FirstOrDefault(p => p.Id == 2)!,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 3,
            StoreId = 1,
            Store = Stores.FirstOrDefault(s => s.Id == 1)!,
            Product = Products.FirstOrDefault(p => p.Id == 3)!,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 4,
            StoreId = 2,
            Store = Stores.FirstOrDefault(s => s.Id == 2)!,
            Product = Products.FirstOrDefault(p => p.Id == 4)!,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 5,
            StoreId = 1,
            Store = Stores.FirstOrDefault(s => s.Id == 1)!,
            Product = Products.FirstOrDefault(p => p.Id == 5)!,
            Quantity = 0,
        },
    };

    public static List<Product> Products => new List<Product>
    {
        new Product(){
            Id = 1,
            Name = "test",
            BrandId = 1,
            CategoryId = 1,
            Price = 100f
        },
        new Product()
        {
            Id = 2,
            Name = "test2",
            BrandId = 1,
            CategoryId = 1,
            Price = 200f
        },
        new Product()
        {
            Id = 3,
            Name = "test3",
            BrandId = 1,
            CategoryId = 1,
            Price = 300f
        },
        new Product()
        {
            Id = 4,
            Name = "test4",
            BrandId = 1,
            CategoryId = 1,
            Price = 400f
        },
        new Product()
        {
            Id = 5,
            Name = "test5",
            BrandId = 1,
            CategoryId = 1,
            Price = 400f
        }
    };

    public static List<Category> Categories => new List<Category>
    {
        new Category()
        {
            Id = 1,
            Name = "test",
            State = true
        }
    };

    public static List<Brand> Brands => new List<Brand> { new Brand() { Id = 1, Name = "test", State = true } };

    public static List<Basket> Baskets => new List<Basket>
    {
        new Basket()
        {
            ApplicationUserId = "2",
            Product = Products[3],
            ProductId = 4,
            Quantity = 1,
            Total = 1,
        },
        new Basket()
        {
            ApplicationUserId = "1",
            Product = Products[0],
            ProductId = 1,
            Quantity = 1,
            Total = 100f,
        },
        new Basket()
        {
            ApplicationUserId = "1",
            Product = Products[1],
            ProductId = 2,
            Quantity = 1,
            Total = 200f,
        },
        new Basket()
        {
            ApplicationUserId = "3",
            Product = null!,
            ProductId = 0,
            Quantity = 0,
            Total = 0f,
        },
    };
}
