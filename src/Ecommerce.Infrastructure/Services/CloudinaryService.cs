using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryOptions _cloudinaryOptions;
    public Cloudinary cloudinary { get; set; }
    private readonly string FOLDER = "Ecommerce";
    public CloudinaryService(IOptions<CloudinaryOptions> cloudinaryOptions)
    {
        _cloudinaryOptions = cloudinaryOptions.Value;
        cloudinary = new Cloudinary(_cloudinaryOptions.GetCloudinaryUrl());
    }

    public async Task<DeletionResult> DeleteImage(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) throw new InvalidOperationException("Image name could not be null or empty");

        var deletionParams = new DeletionParams($"{FOLDER}/{imageName}")
        {
            ResourceType = ResourceType.Image
        };

        return await cloudinary.DestroyAsync(deletionParams);
    }

    public async Task<string> GetImage(string imageName)
    {
        string publicId = $"{FOLDER}/{imageName}";
        var getResourceParams = new GetResourceParams(publicId)
        {
            ResourceType = ResourceType.Image,
            QualityAnalysis = true,
        };

        var getResourceResult = await cloudinary.GetResourceAsync(getResourceParams);
        return getResourceResult.SecureUrl;
    }

    public async Task<(string ImageUrl, string PublicId)> UploadImage(IFormFile file, string imageName)
    {
        var filePath = Path.GetTempFileName();

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var imageUploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath),
            UniqueFilename = false,
            Overwrite = true,
            Folder = FOLDER,
            FilenameOverride = imageName,
            PublicId = imageName
        };

        var imageUploadResult = await cloudinary.UploadAsync(imageUploadParams);

        return (ImageUrl: imageUploadResult.SecureUrl.ToString(), PublicId: imageUploadResult.PublicId);
    }
}