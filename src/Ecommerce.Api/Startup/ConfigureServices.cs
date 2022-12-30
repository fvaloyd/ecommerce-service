using Ecommerce.Api.Filters;

namespace Ecommerce.Api.Startup;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        services.AddSingleton<GlobalFilters>();

        services.AddSwaggerConfiguration(); 

        services.AddControllers(config => {
            config.Filters.Add(typeof(GlobalFilters));
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(ApplicationMappings).Assembly);
        return services;
    }
}
