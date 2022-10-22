using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class OrderTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(Order).Should().BeAssignableTo<BaseEntity>();
    }
}