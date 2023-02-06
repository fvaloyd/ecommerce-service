namespace Ecommerce.Api.IntegrationTests.Controllers.Auth;

[Collection("BaseIntegrationTestCollection")]
public class LogoutTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;
    
    readonly string logoutPath = "api/auth/logout";

    public LogoutTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task ShouldReturnOk_WhenTheUserIsLoged()
    {
        // Act
        var logoutResponse = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(logoutPath, null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}