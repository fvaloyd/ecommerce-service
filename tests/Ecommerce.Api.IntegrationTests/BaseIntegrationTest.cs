using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace Ecommerce.Api.IntegrationTests;

public abstract class BaseIntegrationTest
{
    protected readonly HttpClient _httpClient;
    internal readonly CustomProgram _factory = null!;
    protected readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory = null!;
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly IStripeService _stripeService;
    protected readonly ICloudinaryService _cloudinaryService;
    protected readonly Checkpoint _checkpoint;

    public BaseIntegrationTest()
    {
        _factory = new CustomProgram();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _httpClient = _factory.CreateDefaultClient();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        var scope = _scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _stripeService = scope.ServiceProvider.GetRequiredService<IStripeService>();
        _cloudinaryService = scope.ServiceProvider.GetRequiredService<ICloudinaryService>();
        _checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] { "__EFMigrationsHistory" }
        };
    }
}