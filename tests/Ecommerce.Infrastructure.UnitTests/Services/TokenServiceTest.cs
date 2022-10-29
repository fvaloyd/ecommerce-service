using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Services;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class TokenServiceTest
{
    [Fact]
    public void ShouldImplementITokenService()
    {
        typeof(TokenService).Should().BeAssignableTo<ITokenService>();
    }
}