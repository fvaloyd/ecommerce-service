using Stripe;

namespace Ecommerce.Api.Startup;

public static class ConfigureKeys
{
    public static void SetupApiKeys(WebApplicationBuilder builder)
    {
        // Stripe
        StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["ApiKey"];

        // Sendiblue
        sib_api_v3_sdk.Client.Configuration.Default.ApiKey.Add("api-key", builder.Configuration.GetSection("Smtp")["ApiKey"]);
    }
}