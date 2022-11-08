using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Api.IntegrationTests;

public abstract class BaseIntegrationTest 
{
    protected readonly HttpClient _httpClient;
    internal readonly CustomProgram _factory = null!;
    private readonly IServiceScopeFactory _scopeFactory = null!;
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly IStripeService _stripeService;
    protected readonly ICloudinaryService _cloudinaryService;

    public BaseIntegrationTest()
    {
        _factory = new CustomProgram();
        _httpClient = _factory.CreateDefaultClient();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        var scope = _scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _stripeService = scope.ServiceProvider.GetRequiredService<IStripeService>();
        _cloudinaryService = scope.ServiceProvider.GetRequiredService<ICloudinaryService>();
    }
}