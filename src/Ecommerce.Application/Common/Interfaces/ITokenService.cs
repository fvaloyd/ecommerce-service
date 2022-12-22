using System.Security.Claims;
using Ecommerce.Core.Entities;

namespace Ecommerce.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(UserBase user);
    string CreateRefreshToken();
    ClaimsPrincipal GetPrincipalsFromExpireToken(string expireToken);
}