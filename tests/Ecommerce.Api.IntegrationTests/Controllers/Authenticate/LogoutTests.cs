using Ecommerce.Core.Models;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public class LogoutTests
{
    BaseIntegrationTest _baseIntegrationTest;
    string logoutPath = "api/authenticate/logout";

    public LogoutTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }

    [Fact]
    public async Task ShouldLogOutTheUser()
    {
        var logoutResponse = await _baseIntegrationTest.DefaultUserHttpClient.PostAsync(logoutPath, null);

        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}