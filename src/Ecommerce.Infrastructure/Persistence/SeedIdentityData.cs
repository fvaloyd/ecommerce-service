using Ecommerce.Core.Consts;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence;

public static class SeedIdentityData
{
    const string defaultPassword = "password.123";
    public static async Task Handle(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }

        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        var defaultUser = new ApplicationUser { UserName = "default", Email = "default@gmail.com", PhoneNumber = "8095582534", CustomerId = "cus_MaYQGocnCeaxlC", EmailConfirmed = true};
        await userManager.CreateAsync(defaultUser, defaultPassword);
        defaultUser = await userManager.FindByNameAsync("default");
        await userManager.AddToRoleAsync(defaultUser, UserRoles.User);

        var adminUser = new ApplicationUser { UserName = "admin", Email = "admin@gmail.com", PhoneNumber = "8095582108", CustomerId = "cus_MaYPZimk4Ilx5c", EmailConfirmed = true};
        await userManager.CreateAsync(adminUser, defaultPassword);
        adminUser = await userManager.FindByNameAsync("admin");
        await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
    }
}
