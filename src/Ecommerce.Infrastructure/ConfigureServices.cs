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
using Microsoft.Extensions.Options;
using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Data;
using Ecommerce.Infrastructure.Jwt;
using Ecommerce.Infrastructure.Payment;
using Ecommerce.Infrastructure.Jwt.Options;
using Ecommerce.Infrastructure.Payment.Options;
using Ecommerce.Infrastructure.EmailSender.Options;
using Ecommerce.Infrastructure.CloudImageStorage;
using Ecommerce.Infrastructure.CloudImageStorage.Options;

namespace Ecommerce.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureEcommerceOptions()
                .AddDbContext<EcommerceDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")))
                .ConfigureIdentity()
                .ConfigureJwtAuthentication()
                .SetupServices();

        return services;
    }

    private static IServiceCollection ConfigureEcommerceOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<JWTOptionsSetup>();
        services.ConfigureOptions<StripeSetup>();
        services.ConfigureOptions<CloudinarySetup>();
        services.ConfigureOptions<SmtpSetup>();

        return services;
    }

    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(opt => {
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = true;
            opt.Password.RequiredLength = 9;

            opt.User.RequireUniqueEmail = true;

            opt.SignIn.RequireConfirmedEmail = true;
        })
                .AddEntityFrameworkStores<EcommerceDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }
    
    private static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services)
    {
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

        return services;
    }

    private static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.AddScoped<IEcommerceDbContext>(provider => provider.GetRequiredService<EcommerceDbContext>());

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IStripeService, StripeService>();

        services.AddScoped<ICloudinaryService, CloudinaryService>();

        services.AddScoped<IEmailSender, SendiblueService>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
