namespace Ecommerce.Contracts.Requests;

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string AccessToken, string RefreshToken);

public record RegisterRequest(string UserName, string PhoneNumber, string Email, string Password);