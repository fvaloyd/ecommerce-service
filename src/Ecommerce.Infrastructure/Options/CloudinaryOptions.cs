namespace Ecommerce.Infrastructure.Options;

public class CloudinaryOptions
{
    public string ApiKey { get; set; } = null!;
    public string CloudName { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string GetCloudinaryUrl()
    {
        return $"cloudinary://{this.ApiKey}:{this.ApiSecret}@{this.CloudName}";
    }
}