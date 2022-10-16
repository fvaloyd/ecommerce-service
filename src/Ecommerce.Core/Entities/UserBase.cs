using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.Entities;

public class UserBase
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}