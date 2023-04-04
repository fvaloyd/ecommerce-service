using Ecommerce.Contracts;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;
using Ecommerce.Api.IntegrationTests;

[Collection("BaseIntegrationTestCollection")]
public class AuthControllerTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    public AuthControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task Login_ShouldReturnAnAuthenticationResponse_WhenTheUserIsAuthenticate()
    {
        // Arrange
        var authenticatedUser = new LoginRequest("admin@gmail.com", "password.123");

        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.Login, authenticatedUser);

        var stringResult = await httpResponse.Content.ReadAsStringAsync();

        var parseAuthenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(stringResult);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        parseAuthenticateResponse.AccessToken.Should().NotBeNull();

        parseAuthenticateResponse.AccessToken.Should().NotBeEmpty();

        parseAuthenticateResponse.RefreshToken.Should().NotBeNull();

        parseAuthenticateResponse.RefreshToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorize_WhenUnAuthorizeUserTryToLogin()
    {
        // Arrange
        var unAuthenticatedUser = new LoginRequest("invalid@gmail.com", "invalid.123");
        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.Login, unAuthenticatedUser);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenIncorrectPasswordIsSent()
    {
        // Arrange
        var authenticatedUserWithIncorrectPassword = new LoginRequest("admin@gmail.com", "password");
        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.Login, authenticatedUserWithIncorrectPassword);
        
        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenTheUserIsLoged()
    {
        // Act
        var logoutResponse = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(ApiRoutes.Auth.Logout, null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnForbidden_WhenNoAdminUserTriesToCreateAnAdminUser()
    {
        // Arrange
        var ValidUser = new RegisterRequest("registeradmintest", "8888888888", "registeradmintest@gmail.com", "test.324234");

        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(ApiRoutes.Auth.RegisterAdmin, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnUnAuthorize_UnAuthenticateUserTriesToCreateAnAdministrator()
    {
        // Arrange
        var ValidUser = new RegisterRequest("registeradmintest", "8888888888", "registeradmintest@gmail.com", "test.324234");

        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.RegisterAdmin, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnOk_WhenAdminUserTriesToRegisterAnValidUser()
    {
        // Arrange
        var ValidUser = new RegisterRequest("registeradmintest", "8888888888", "registeradmintest@gmail.com", "test.324234");

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Auth.RegisterAdmin, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnBadRequest_WhenAdminUserTriesToRegisterAnAlreadyAuthenticateUser()
    {
        // Arrange
        var ExistingUser = new RegisterRequest("admin", "8888888888", "admin@gmail.com", "password.123");

        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(ApiRoutes.Auth.RegisterAdmin, ExistingUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenValidUserIsSending()
    {
        // Arrange
        var ValidUser = new RegisterRequest("registertest", "8888888888", "registertest@gmail.com", "test.123324234");

        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.Register, ValidUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenExistingUserIsSending()
    {
        // Arrange
        var ExistingUser = new RegisterRequest("admin", "8888888888", "admin@gmail.com", "password.123");

        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(ApiRoutes.Auth.Register, ExistingUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}