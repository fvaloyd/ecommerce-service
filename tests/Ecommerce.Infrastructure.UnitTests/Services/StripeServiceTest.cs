using Ecommerce.Infrastructure.Services;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class StripeServiceTest
{
    [Fact]
    public void ShouldImplementIStripeService()
    {
        typeof(StripeService).Should().BeAssignableTo<IStripeService>();
    }
}