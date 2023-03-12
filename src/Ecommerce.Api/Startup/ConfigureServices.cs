using Ecommerce.Api.Filters;
using Hangfire;
using Hangfire.SqlServer;

namespace Ecommerce.Api.Startup;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ApiExceptionFilter>();

        services.AddSwaggerConfiguration(); 

        services.AddControllers(config => {
            config.Filters.Add(typeof(ApiExceptionFilter));
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(ApplicationMappings).Assembly);

        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
        });

        services.AddHangfireServer();

        return services;
    }
}
