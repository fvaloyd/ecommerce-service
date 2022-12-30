using Ecommerce.Infrastructure.Payment;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class StripeServiceTest
{
    [Fact]
    public void ShouldImplementIStripeService()
    {
        typeof(StripeService).Should().BeAssignableTo<IStripeService>();
    }
}