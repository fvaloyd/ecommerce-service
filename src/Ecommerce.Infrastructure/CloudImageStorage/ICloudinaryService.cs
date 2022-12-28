using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Infrastructure.CloudImageStorage;

public interface ICloudinaryService
{
    Task<(string ImageUrl, string PublicId)> UploadImage(IFormFile file, string imageName);
    Task<string> GetImage(string imageName);
    Task<DeletionResult> DeleteImage(string imageName);
}