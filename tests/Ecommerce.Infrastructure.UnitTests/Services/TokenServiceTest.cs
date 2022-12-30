using Ecommerce.Infrastructure.Jwt;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class TokenServiceTest
{
    [Fact]
    public void ShouldImplementITokenService()
    {
        typeof(TokenService).Should().BeAssignableTo<ITokenService>();
    }
}