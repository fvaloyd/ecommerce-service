using Ecommerce.Core.Models;
namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public class ResgisterAdminTest
{
    BaseIntegrationTest _baseIntegrationTest;

    public ResgisterAdminTest(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    string endPointPath = "/api/authenticate/register-admin";
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
    public async Task NonAdministratorUserTriesToCreateAnAdministrator_ShouldReturnForbidden()
    {
        var response = await _baseIntegrationTest.DefaultUserHttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ValidUser);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UnAuthenticateUserTriesToCreateAnAdministrator_ShouldReturnUnAuthorize()
    {
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ValidUser);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminUserTriesToRegisterAnValidUser_ShouldReturnOk()
    {
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ValidUser);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminUserTriesToRegisterAnAlreadyAuthenticateUser_ShouldReturnBadRequest()
    {
        var response = await _baseIntegrationTest.AdminUserHttpClient.PostAsJsonAsync<RegisterUser>(endPointPath, ExistingUser);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}