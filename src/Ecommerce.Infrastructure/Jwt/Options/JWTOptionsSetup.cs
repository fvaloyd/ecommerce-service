using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Jwt.Options;

public class JWTOptionsSetup : IConfigureOptions<JWTOptions>
{
    private readonly IConfiguration _configuration;
    private const string ConfigurationSectionName = "JWT";

    public JWTOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JWTOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}