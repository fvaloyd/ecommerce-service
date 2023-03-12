using Ecommerce.Core.Enums;
using Ecommerce.Infrastructure.Jwt;
using Ecommerce.Infrastructure.Payment;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Api.BackgroundJobs;
using Ecommerce.Contracts.Authentication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Hangfire;

namespace Ecommerce.Api.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ITokenService _tokenService;
    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IStripeService stripeService,
        SignInManager<ApplicationUser> signInManager,
        IBackgroundJobClient backgroundJobClient)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _stripeService = stripeService;
        _signInManager = signInManager;
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        ApplicationUser? userExist = await _userManager.FindByEmailAsync(registerRequest.Email);

        if (userExist != null) return BadRequest("User already exist");

        ApplicationUser user = new()
        {
            Email = registerRequest.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerRequest.UserName,
            PhoneNumber = registerRequest.PhoneNumber,
        };

        var customer = await _stripeService.CreateCustomerToken(user);
        user.CustomerId = customer.Id;

        IdentityResult result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded) return BadRequest("User creation failed! Please try again");

        await _userManager.AddToRoleAsync(user, UserRoles.User);

        _backgroundJobClient.Enqueue<SendMailToConfirmEmail>(sm => sm.Handle(user.Id, Request.Scheme, Request.Host.Value));

        return Ok("Check you mail message and confirm your email");
    }

    [HttpGet("confirm-email", Name = "ConfirmEmail")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) return NotFound("User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded) return BadRequest("Could not confirm the email");

        return Ok("Email confirm successfully");
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest model)
    {
        ApplicationUser? userExist = await _userManager.FindByEmailAsync(model.Email);

        if (userExist != null) return BadRequest("User already exist");

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        var customer = await _stripeService.CreateCustomerToken(user);

        user.CustomerId = customer.Id;

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return BadRequest("Error occurred creating the user");

        await _userManager.AddToRoleAsync(user, UserRoles.Admin);

        _backgroundJobClient.Enqueue<SendMailToConfirmEmail>(sm => sm.Handle(user.Id, Request.Scheme, Request.Host.Value));

        return Ok("User created successfully");
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null) return Unauthorized();

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

        if (!isEmailConfirmed)
        {
            _backgroundJobClient.Enqueue<SendMailToConfirmEmail>(sm => sm.Handle(user.Id, Request.Scheme, Request.Host.Value));

            return BadRequest("You need to confirm your email. Check your mail to confirm");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (!signInResult.Succeeded) return BadRequest("Incorrect password");

        string accessToken = await _tokenService.CreateToken(user);

        string refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;

        user.RefreshTokenExpireTime = DateTime.Now.AddHours(5);

        await _userManager.UpdateAsync(user);

        return Ok(new AuthenticateResponse(AccessToken: accessToken, RefreshToken: refreshToken));
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();

        return Ok("Closed session");
    }
}