using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

public class RegisterTests : BaseIntegrationTest, IDisposable
{
    private ApplicationUser userCreated = null!;
    private string endPointPath = "/api/authenticate/register";
    private RegisterUser ValidUser = new() {
        Email = "test@gmail.com",
        Password = "test.123324234",
        UserName = "test",
        PhoneNumber = "8888888888"
    };
    private Object ExistingUser = new {
        Username = "admin",
        PhoneNumber = "8888888888",
        Email = "admin@gmail.com",
        Password = "password.123"
    };

    [Fact]
    public async Task Register_WithValidUser_ShouldReturnOk()
    {
        var response = await _httpClient.PostAsJsonAsync(endPointPath, ValidUser);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userCreated = await _userManager.FindByEmailAsync(ValidUser.Email);
    }

    [Fact]
    public async Task Register_WithExistingUser_ShouldReturnBadRequest()
    {
        var response = await _httpClient.PostAsJsonAsync(endPointPath, ExistingUser);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public void Dispose()
    {
        if (userCreated is null) return;
        _stripeService.DeleteCustomer(userCreated);
        _checkpoint.Reset(_configuration.GetConnectionString("TestConnection")).Wait();
    }
}