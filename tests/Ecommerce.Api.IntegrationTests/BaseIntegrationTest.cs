using Ecommerce.Api.IntegrationTests.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace Ecommerce.Api.IntegrationTests;

public class BaseIntegrationTest : IAsyncLifetime
{
    public HttpClient HttpClient = null!;
    internal CustomProgram _factory = null!;
    private IConfiguration _configuration = null!;
    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        _factory = new CustomProgram();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        HttpClient = _factory.CreateDefaultClient();
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

    public async Task DisposeAsync()
    {
        await _respawner.ResetAsync(_configuration.GetConnectionString("TestConnection"));
    }
}