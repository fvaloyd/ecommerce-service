using Ecommerce.Core.Entities;
using FluentAssertions;
using Xunit;

namespace Ecommerce.Core.UnitTests.Entities;

public class ProductTest
{
    [Fact]
    public void Should_inherit_BaseEntity()
    {
        typeof(Product).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void ChangePrice_should_change_the_price()
    {
        float newPrice = 199.99f;
        Product productMock = new("product1", 100, 1, 1, "url.com");

        productMock.ChangePrice(newPrice);

        productMock.Price.Should().Be(newPrice);
    }
}