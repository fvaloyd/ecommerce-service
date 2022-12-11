using Ecommerce.Core.Entities;

namespace Ecommerce.Application.UnitTests;
static class TestData
{
    public static Store[] Stores => new Store[]
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
    };

    public static ProductStore[] ProductStores => new ProductStore[]
    {
        new ProductStore
        {
            ProductId = 1,
            StoreId = 1,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 2,
            StoreId = 1,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 3,
            StoreId = 1,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 4,
            StoreId = 2,
            Quantity = 100,
        },
        new ProductStore
        {
            ProductId = 5,
            StoreId = 1,
            Quantity = 0,
        },

    };

    public static Product[] Products => new Product[]
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
        }
    };

    public static Category[] Categories => new Category[]
    {
        new Category()
        {
            Id = 1,
            Name = "test",
            State = true
        }
    };

    public static Brand[] Brands => new Brand[] { new Brand() { Id = 1, Name = "test", State = true } };

    public static Basket[] Baskets => new Basket[]
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
    };
}
