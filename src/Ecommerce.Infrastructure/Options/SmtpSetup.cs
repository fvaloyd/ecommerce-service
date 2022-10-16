using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Options;

public class SmtpSetup : IConfigureOptions<SmtpOptions>
{
    private const string ConfigurationSectionName = "Smtp";
    private readonly IConfiguration _configuration;

    public SmtpSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(SmtpOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}