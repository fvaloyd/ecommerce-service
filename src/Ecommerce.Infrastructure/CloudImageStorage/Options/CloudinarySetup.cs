using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.CloudImageStorage.Options;

public class CloudinarySetup : IConfigureOptions<CloudinaryOptions>
{
    private readonly IConfiguration _configuration;
    private const string ConfigurationSection = "Cloudinary";

    public CloudinarySetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(CloudinaryOptions options)
    {
        _configuration.GetSection(ConfigurationSection).Bind(options);
    }
}