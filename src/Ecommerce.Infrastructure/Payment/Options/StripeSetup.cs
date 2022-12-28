using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Payment.Options;

public class StripeSetup : IConfigureOptions<StripeOptions>
{
    private readonly IConfiguration _configuration;
    private const string ConfigurationSectionName = "Stripe";

    public StripeSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(StripeOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}
