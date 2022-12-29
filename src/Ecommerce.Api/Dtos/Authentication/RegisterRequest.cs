namespace Ecommerce.Api.Dtos.Authentication;
public record RegisterRequest(string UserName, string PhoneNumber, string Email, string Password);