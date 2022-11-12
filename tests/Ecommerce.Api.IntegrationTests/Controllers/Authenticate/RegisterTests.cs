using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public class RegisterTests
{
    BaseIntegrationTest _baseIntegrationTest;
    public RegisterTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }
    string endPointPath = "/api/authenticate/register";
    RegisterUser ValidUser = new() {
        Email = "test@gmail.com",
        Password = "test.123324234",
        UserName = "test",
        PhoneNumber = "8888888888"
    };
    RegisterUser ExistingUser = new() {
        UserName = "admin",
        PhoneNumber = "8888888888",
        Email = "admin@gmail.com",
        Password = "password.123"
    };

    [Fact]
    public async Task Register_WithValidUser_ShouldReturnOk()
    {
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ValidUser);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithExistingUser_ShouldReturnBadRequest()
    {
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ExistingUser);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}