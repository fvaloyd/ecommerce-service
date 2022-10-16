namespace Ecommerce.Api.Startup;

public static class DependencyInjectionSetup
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSwaggerConfiguration(); 

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(ApplicationMappings).Assembly);
        return services;
    }
}
