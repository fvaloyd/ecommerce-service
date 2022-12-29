using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Identity;

using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.Startup;

public static class ApplicationSeedData
{
    public static async Task<WebApplication> SeedDataHandle(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var db = serviceProvider.GetRequiredService<EcommerceDbContext>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedData.Handle(db, app.Logger);
                await SeedIdentityData.Handle(db, userManager, roleManager);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "An error ocurred seeding data");
            }
        }

        return app;
    }
}
