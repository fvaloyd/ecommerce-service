using Ecommerce.Api.IntegrationTests.Startup;
using Ecommerce.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace Ecommerce.Api.IntegrationTests;

public class BaseIntegrationTest : IAsyncLifetime
{
    LoginUser defaultUser = new()
    {
        Email = "default@gmail.com",
        Password = "password.123"
    };
    LoginUser adminUser = new()
    {
        Email = "admin@gmail.com",
        Password = "password.123"
    };
    public HttpClient HttpClient = null!;
    public HttpClient AdminUserHttpClient = null!;
    public HttpClient DefaultUserHttpClient = null!;
    internal CustomProgram _factory = null!;
    private IConfiguration _configuration = null!;
    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        _factory = new CustomProgram();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        HttpClient = _factory.CreateDefaultClient();
        AdminUserHttpClient = await GetCustomHttpClient(_factory, HttpClient, adminUser);
        DefaultUserHttpClient = await GetCustomHttpClient(_factory, HttpClient, defaultUser);
        _respawner = await Respawner.CreateAsync(_configuration.GetConnectionString("TestConnection"), new RespawnerOptions{
            TablesToIgnore = new Table[]
            {
                "AspNetRoleClaims",
                "AspNetRoles",
                "AspNetUserClaims",
                "AspNetUserLogins",
                "AspNetUserRoles",
                "AspNetUserTokens",
                "__EFMigrationsHistory"
            }
        });
    }

    private async Task<HttpClient> GetCustomHttpClient(CustomProgram program, HttpClient httpClient, LoginUser user)
    {
        var httpResponse = await httpClient.PostAsJsonAsync<LoginUser>("api/authenticate/login", user);
        var httpResponseReadedAsString = await httpResponse.Content.ReadAsStringAsync();
        var authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(httpResponseReadedAsString);

        var customHttpClient = program.CreateDefaultClient();
        customHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticateResponse.AccessToken);
        return customHttpClient;
    }

    public async Task DisposeAsync()
    {
        await _respawner.ResetAsync(_configuration.GetConnectionString("TestConnection"));
    }
}