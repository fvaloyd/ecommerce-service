using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ecommerce.Api.IntegrationTests;

internal class CustomProgram : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder => {
            var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) => {
            services.Remove<DbContextOptions<ApplicationDbContext>>()
                    .AddDbContext<ApplicationDbContext>((sp, options) => {
                        options.UseSqlServer(builder.Configuration.GetConnectionString("TestConnection"),
                            builder => builder.MigrationsAssembly(typeof(Ecommerce.Infrastructure.AssemblyReference).Assembly.FullName));
            });
        });

        return base.CreateHost(builder);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Remove<TService>(this IServiceCollection service)
    {
        var serviceDescriptor = service.FirstOrDefault(d => d.ServiceType == typeof(TService));

        if (serviceDescriptor is not null)
        {
            service.Remove(serviceDescriptor);
        }

        return service;
    }
}