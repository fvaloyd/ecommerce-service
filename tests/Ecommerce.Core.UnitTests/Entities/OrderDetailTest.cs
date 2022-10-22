using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class OrderDetailTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(OrderDetail).Should().BeAssignableTo<BaseEntity>();
    }
}