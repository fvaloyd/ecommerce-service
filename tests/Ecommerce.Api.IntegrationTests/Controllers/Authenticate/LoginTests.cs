using Ecommerce.Core.Models;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

public sealed class LoginTests : BaseIntegrationTest
{
    private string endPointPath = "/api/authenticate/login";
    private Object authenticatedUser = new {
        Email = "admin@gmail.com",
        Password = "password.123"
    };
    private Object authenticatedUserWithIncorrectPassword = new {
        Email = "admin@gmail.com",
        Password = "password"
    };
    private Object unAuthenticatedUser = new {
        Email = "invalid@gmail.com",
        Password = "invalid.123"
    };

    [Fact]
    public async Task Login_WithAuthenticatedUser_ShouldReturnAnAuthenticationResponse()
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(endPointPath, authenticatedUser);

        var stringResult = await httpResponse.Content.ReadAsStringAsync();

        var parseAuthenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(stringResult);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        parseAuthenticateResponse.AccessToken.Should().NotBeNull();
        parseAuthenticateResponse.AccessToken.Should().NotBeEmpty();
        parseAuthenticateResponse.RefreshToken.Should().NotBeNull();
        parseAuthenticateResponse.RefreshToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_WithUnAuthenticateUser_ShouldReturnUnauthorize()
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(endPointPath, unAuthenticatedUser);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_ShouldReturnBadRequest()
    {
        var httpResponse = await _httpClient.PostAsJsonAsync(endPointPath, authenticatedUserWithIncorrectPassword);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}