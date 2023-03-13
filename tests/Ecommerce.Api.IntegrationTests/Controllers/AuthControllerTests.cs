using Ecommerce.Api.IntegrationTests;
using Ecommerce.Contracts.Authentication;
using Ecommerce.Contracts.Endpoints;

[Collection("BaseIntegrationTestCollection")]
public class AuthControllerTests
{
    const string ApiRoot = "api/";
    const string LoginPath = $"{ApiRoot}{AuthEndpoints.Login}";
    const string LogoutPath = $"{ApiRoot}{AuthEndpoints.Logout}";
    const string RegisterAdminPath = $"{ApiRoot}{AuthEndpoints.RegisterAdmin}";
    const string RegisterPath = $"{ApiRoot}{AuthEndpoints.Register}";

    readonly RegisterRequest ValidUser = new("registeradmintest", "8888888888", "registeradmintest@gmail.com", "test.324234");

    readonly RegisterRequest ExistingUser = new("admin", "8888888888", "admin@gmail.com", "password.123"); 

    readonly LoginRequest authenticatedUser = new("admin@gmail.com", "password.123");

    readonly LoginRequest authenticatedUserWithIncorrectPassword = new("admin@gmail.com", "password");

    readonly LoginRequest unAuthenticatedUser = new("invalid@gmail.com", "invalid.123"); 

    readonly BaseIntegrationTest _baseIntegrationTest;

    public AuthControllerTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task Login_ShouldReturnAnAuthenticationResponse_WhenTheUserIsAuthenticate()
    {
        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(LoginPath, authenticatedUser);

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
        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(LoginPath, unAuthenticatedUser);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenIncorrectPasswordIsSent()
    {
        // Act
        var httpResponse = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(LoginPath, authenticatedUserWithIncorrectPassword);
        
        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenTheUserIsLoged()
    {
        // Act
        var logoutResponse = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(LogoutPath, null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnForbidden_WhenNoAdminUserTriesToCreateAnAdminUser()
    {
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(RegisterAdminPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnUnAuthorize_UnAuthenticateUserTriesToCreateAnAdministrator()
    {
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(RegisterAdminPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnOk_WhenAdminUserTriesToRegisterAnValidUser()
    {
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(RegisterAdminPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterAdmin_ShouldReturnBadRequest_WhenAdminUserTriesToRegisterAnAlreadyAuthenticateUser()
    {
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(RegisterAdminPath, ExistingUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenValidUserIsSending()
    {
        RegisterRequest ValidUser = new("registertest", "8888888888", "registertest@gmail.com", "test.123324234");
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(RegisterPath, ValidUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenExistingUserIsSending()
    {
        RegisterRequest ExistingUser = new("admin", "8888888888", "admin@gmail.com", "password.123"); 
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(RegisterPath, ExistingUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}