using Ecommerce.Api.Dtos.Authentication;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public sealed class LoginTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    readonly string endPointPath = "/api/authenticate/login";

    public LoginTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    readonly LoginRequest authenticatedUser = new("admin@gmail.com", "password.123");

    readonly LoginRequest authenticatedUserWithIncorrectPassword = new("admin@gmail.com", "password");

    readonly LoginRequest unAuthenticatedUser = new("invalid@gmail.com", "invalid.123"); 

    [Fact]
    public async Task Login_WithAuthenticatedUser_ShouldReturnAnAuthenticationResponse()
    {
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, authenticatedUser);

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
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, unAuthenticatedUser);

        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_ShouldReturnBadRequest()
    {
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, authenticatedUserWithIncorrectPassword);
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}