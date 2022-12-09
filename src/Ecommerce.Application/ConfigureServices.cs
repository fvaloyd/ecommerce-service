using Ecommerce.Application.Baskets;
using Ecommerce.Application.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBasketService, BasketService>()
                .AddScoped<IStoreService, StoreService>()
                .AddScoped<IProductService, ProductService>();

        return services;
    }
}