namespace Ecommerce.Contracts.Authentication;
public record AuthenticateResponse(string AccessToken, string RefreshToken);