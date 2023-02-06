using Ecommerce.Api.Dtos.Authentication;
namespace Ecommerce.Api.IntegrationTests.Controllers.Auth;

[Collection("BaseIntegrationTestCollection")]
public class ResgisterAdminTest
{
    readonly BaseIntegrationTest _baseIntegrationTest;

    public ResgisterAdminTest(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    readonly string endPointPath = "/api/auth/register-admin";

    readonly RegisterRequest ValidUser = new("registeradmintest", "8888888888", "registeradmintest@gmail.com", "test.324234");

    readonly RegisterRequest ExistingUser = new("admin", "8888888888", "admin@gmail.com", "password.123"); 

    [Fact]
    public async Task ShouldReturnForbidden_WhenNoAdminUserTriesToCreateAnAdminUser()
    {
        // Act
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync(endPointPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShouldReturnUnAuthorize_UnAuthenticateUserTriesToCreateAnAdministrator()
    {
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenAdminUserTriesToRegisterAnValidUser()
    {
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath, ValidUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenAdminUserTriesToRegisterAnAlreadyAuthenticateUser()
    {
        // Act
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync(endPointPath, ExistingUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}