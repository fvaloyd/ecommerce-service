using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Infrastructure.Identity;
public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public string? CustomerId { get; set; }
}