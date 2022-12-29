using Ecommerce.Api.Dtos.Authentication;

namespace Ecommerce.Api.IntegrationTests.Controllers.Authenticate;

[Collection("BaseIntegrationTestCollection")]
public class RegisterTests
{
    readonly BaseIntegrationTest _baseIntegrationTest;
    
    public RegisterTests(BaseIntegrationTest baseIntegrationTest)
    {
        _baseIntegrationTest = baseIntegrationTest;
    }
    
    readonly string endPointPath = "/api/authenticate/register";

    readonly RegisterRequest ValidUser = new("registertest", "8888888888", "registertest@gmail.com", "test.123324234");

    readonly RegisterRequest ExistingUser = new("admin", "8888888888", "admin@gmail.com", "password.123"); 

    [Fact]
    public async Task Register_ShouldReturnOk_WhenValidUserIsSending()
    {
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, ValidUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenExistingUserIsSending()
    {
        // Act
        var response = await _baseIntegrationTest.HttpClient.PostAsJsonAsync(endPointPath, ExistingUser);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}