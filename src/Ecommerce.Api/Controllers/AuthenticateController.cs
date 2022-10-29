using System.Net;
using System.Reflection;
using Ecommerce.Core.Consts;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

public class AuthenticateController : ApiControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailService;
    public AuthenticateController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IStripeService stripeService,
        IEmailSender emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _stripeService = stripeService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser model)
    {
        ApplicationUser userExist = await _userManager.FindByEmailAsync(model.Email);

        if (userExist != null) return StatusCode(StatusCodes.Status500InternalServerError, new Response(Status:HttpStatusCode.InternalServerError, Message: "User already exist"));

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            PhoneNumber = model.PhoneNumber,
        };

        var customer = await _stripeService.CreateCustomerToken(user);
        user.CustomerId = customer.Id;

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new Response(Status:HttpStatusCode.InternalServerError, Message:"User creation failed! Please try again"));

        await _userManager.AddToRoleAsync(user, UserRoles.User);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authenticate", new { token, email = user.Email}, Request.Scheme);

        var mailRequest = await CreateMailRequest(user, confirmationLink!);
        await _emailService.SendAsync(mailRequest);

        return Ok(new Response(Status: HttpStatusCode.OK, Message: "Check you mail message and confirm your email"));
    }

    [HttpGet("confirm-email", Name = "ConfirmEmail")]
    public async Task<ActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return NotFound("User not found");
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return BadRequest("Could not confirm the email");
        return Ok("Email confirm successfully");
    }

    private async Task<MailRequest> CreateMailRequest(ApplicationUser user, string confirmationLink)
    {
        var emailConfirmationMailModel = new EmailConfirmationMailModel(User: user, ConfirmationLink: confirmationLink);
        string template = await _emailService.GetTemplate(((int)MailTemplates.EmailConfirmation));
        string compiledTemplate = await _emailService.GetCompiledTemplateAsync(template, emailConfirmationMailModel);

        return new MailRequest(
            Body: compiledTemplate,
            Subject: "Email confirmation",
            Email: user.Email
        );
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUser model)
    {
        ApplicationUser userExist = await _userManager.FindByEmailAsync(model.Email);

        if (userExist != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new Response( Status: HttpStatusCode.InternalServerError, Message: "User already exist"));

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName
        };

        var customer = await _stripeService.CreateCustomerToken(user);
        user.CustomerId = customer.Id;

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        await _userManager.AddToRoleAsync(user, UserRoles.Admin);

        return Ok(new Response(Status: HttpStatusCode.OK, Message: "User created successfully"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser model)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

        if (!isEmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authenticate", new { token, email = user.Email}, Request.Scheme);

            var mailRequest = await CreateMailRequest(user, confirmationLink!);
            await _emailService.SendAsync(mailRequest);
            return BadRequest("You need to confirm your email");
        }

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            string token = await _tokenService.CreateToken(model);
            string RefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpireTime = DateTime.Now.AddHours(3);
            await _userManager.UpdateAsync(user);


            return Ok(new AuthenticateResponse(AccessToken: token, RefreshToken: RefreshToken));
        }
        return Unauthorized();
    }
}