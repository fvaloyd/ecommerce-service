using Ecommerce.Infrastructure.Services;

namespace Ecommerce.Infrastructure.UnitTests.Services;

public class CloudinaryServiceTest
{
    [Fact]
    public void ShouldImplementICloudinaryService()
    {
        typeof(CloudinaryService).Should().BeAssignableTo<ICloudinaryService>();
    }
}