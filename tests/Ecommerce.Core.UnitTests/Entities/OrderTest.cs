using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class OrderTest
{
    [Fact]
    public void ShouldInheritBaseEntity()
    {
        typeof(Order).Should().BeAssignableTo<BaseEntity>();
    }
}