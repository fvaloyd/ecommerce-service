using System.Security.Claims;
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.Jwt;

public interface ITokenService
{
    Task<string> CreateToken(ApplicationUser user);
    string CreateRefreshToken();
    ClaimsPrincipal GetPrincipalsFromExpireToken(string expireToken);
}