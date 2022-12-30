namespace Ecommerce.Infrastructure.CloudImageStorage.Options;

public class CloudinaryOptions
{
    public string ApiKey { get; set; } = null!;
    public string CloudName { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string GetCloudinaryUrl()
    {
        return $"cloudinary://{ApiKey}:{ApiSecret}@{CloudName}";
    }
}