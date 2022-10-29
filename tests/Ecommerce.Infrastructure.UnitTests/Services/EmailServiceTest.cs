using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Services;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class EmailServiceTest
{
    [Fact]
    public void ShouldImplementIEmailService()
    {
        typeof(EmailService).Should().BeAssignableTo<IEmailService>();
    }
}