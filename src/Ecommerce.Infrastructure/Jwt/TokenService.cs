using Ecommerce.Infrastructure.Identity;
using Ecommerce.Core.Enums;
using Ecommerce.Infrastructure.Jwt.Options;

using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Infrastructure.Jwt;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JWTOptions _jwtOptions;

    public TokenService(UserManager<ApplicationUser> userManager, IOptions<JWTOptions> jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public string CreateRefreshToken()
    {
        var randNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randNumber);
            return Convert.ToBase64String(randNumber);
        }
    }

    public async Task<string> CreateToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var isAdmin = userRoles.Any(r => r == UserRoles.Admin);
        if (isAdmin)
            authClaims.Add(new Claim(ClaimTypes.Role, UserRoles.Admin));
        if (!isAdmin)
            authClaims.Add(new Claim(ClaimTypes.Role, UserRoles.User));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal GetPrincipalsFromExpireToken(string expireToken)
    {
        var tokenValidationParameter = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _jwtOptions.ValidAudience,
            ValidIssuer = _jwtOptions.ValidIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(expireToken, tokenValidationParameter, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}