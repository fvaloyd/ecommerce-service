using System.Text;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ecommerce.Infrastructure.Services;
using Ecommerce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Data;

namespace Ecommerce.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Options
        services.ConfigureOptions<JWTOptionsSetup>();
        services.ConfigureOptions<StripeSetup>();
        services.ConfigureOptions<CloudinarySetup>();
        services.ConfigureOptions<SmtpSetup>();

        // Context
        services.AddDbContext<EcommerceDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(opt => {
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = true;
            opt.Password.RequiredLength = 9;

            opt.User.RequireUniqueEmail = true;

            opt.SignIn.RequireConfirmedEmail = true;
        })
                .AddEntityFrameworkStores<EcommerceDbContext>()
                .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {

            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var jwtOptions = serviceProvider.GetService<IOptions<JWTOptions>>()!.Value;

            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = jwtOptions.ValidAudience,
                ValidIssuer = jwtOptions.ValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };
        });

        // Services
        services.AddScoped<IEcommerceDbContext>(provider => provider.GetRequiredService<EcommerceDbContext>());

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IStripeService, StripeService>();

        services.AddScoped<ICloudinaryService, CloudinaryService>();

        services.AddScoped<IEmailSender, SendiblueService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
