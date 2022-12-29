namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public class LogoutTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;
    
    readonly string logoutPath = "api/authenticate/logout";

    public LogoutTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task ShouldReturnOk_WhenAlreadyAUserLogIn()
    {
        // Act
        var logoutResponse = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(logoutPath, null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}