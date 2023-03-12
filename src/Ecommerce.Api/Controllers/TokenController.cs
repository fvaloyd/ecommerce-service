using Ecommerce.Infrastructure.Jwt;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Contracts.Authentication;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Api.Controllers;

public class TokenController : ApiControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenController(
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpPost("refresh",Name = "refresh")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest apiToken)
    {
        if (apiToken is null) return BadRequest("Invalid apiToken");

        ClaimsPrincipal userPrincipal = _tokenService.GetPrincipalsFromExpireToken(apiToken.AccessToken);

        string userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        ApplicationUser? user = await _userManager.FindByIdAsync(userId);

        if (user is null || user.RefreshToken != apiToken.RefreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
            return BadRequest("Something go wrong with user token");

        string newAccessToken = await _tokenService.CreateToken(user);
        string newRefreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpireTime = DateTime.Now.AddHours(3);

        await _userManager.UpdateAsync(user);

        return Ok(new AuthenticateResponse(RefreshToken: newRefreshToken, AccessToken: newAccessToken));
    }

    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke()
    {
        ClaimsPrincipal userPrincipal = HttpContext.User;
        string userEmail = userPrincipal.FindFirst(ClaimTypes.Email)!.Value;

        ApplicationUser? currentUser = await _userManager.FindByEmailAsync(userEmail);

        if (currentUser is null) return BadRequest();

        currentUser.RefreshToken = null;

        await _userManager.UpdateAsync(currentUser);

        return NoContent();
    }
}