using System.ComponentModel.DataAnnotations;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Models;

public class RegisterUser : UserBase
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string PhoneNumber { get; set; } = null!;
}