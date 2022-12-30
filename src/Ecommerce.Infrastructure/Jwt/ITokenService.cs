using Ecommerce.Infrastructure.Identity;

using System.Security.Claims;

namespace Ecommerce.Infrastructure.Jwt;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
    string CreateRefreshToken();
    ClaimsPrincipal GetPrincipalsFromExpireToken(string expireToken);
}