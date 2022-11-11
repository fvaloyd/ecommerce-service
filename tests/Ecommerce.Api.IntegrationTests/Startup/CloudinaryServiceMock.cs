using CloudinaryDotNet.Actions;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Api.IntegrationTests.Startup;

public class CloudinaryServiceMock : ICloudinaryService
{
    public Task<DeletionResult> DeleteImage(string imageName)
    {
        return Task.FromResult(new DeletionResult());
    }

    public Task<string> GetImage(string imageName)
    {
        return Task.FromResult("https://testimage.com");
    }

    public Task<(string ImageUrl, string PublicId)> UploadImage(IFormFile file, string imageName)
    {
        return Task.FromResult(("https://testimage.com", "123"));
    }
}
