using Ecommerce.Core.Entities;
using Xunit;

namespace Ecommerce.Core.UnitTests;

public class UnitTest1
{
    Product product = new()
    {
        Id = 1,
        BrandId = 1,
        CategoryId = 1,
        ImageUrl = "",
        Name = "product1",
        Price = 200
    };

    [Fact]
    public void Should_change_the_productprice()
    {
        // Arrange
        float newPrice = 300;
        Product product = new()
        {
            Id = 1,
            BrandId = 1,
            CategoryId = 1,
            ImageUrl = "",
            Name = "product1",
            Price = 200
        };

        // Act
        product.ChangePrice(newPrice);

        // Assert
        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void Should_throw_exception_if_name_is_empty()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Product2(name: ""));
        Assert.Throws<ArgumentException>(() => new Product2(name: null!));
        Assert.Equal("The of the product could not be null or empty", ex.Message);
    }

    [Fact]
    public void Should_return_corresponding_sum()
    {
        Assert.Equal(4, Add(2, 2));
    }

    [Fact]
    public void Should_throw_exception_if_params_zero()
    {
        var ex = Assert.Throws<ArgumentException>(() => Add(0,0));
        Assert.Equal("The two parameter could not be zero", ex.Message);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(9)]
    public void Should_return_true_if_value_is_odd(int value)
    {
        Assert.True(IsOdd(value));
    }

    int Add(int a, int b)
    {
        if (a == 0 && b ==0)
        {
            throw new ArgumentException("The two parameter could not be zero");
        }
        return a + b;
    }

    bool IsOdd(int value)
    {
        return value % 2 == 1;
    }

    class Product2
    {
        private string Name { get; set; } = null!;
        public Product2(string name) => setName(name);
        private void setName(string name)
        {
            if (name == null || name.Count() < 1)
            {
                throw new ArgumentException("The of the product could not be null or empty");
            }
            this.Name = name;
        }
    }
}