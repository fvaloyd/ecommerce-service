namespace Ecommerce.Contracts.Authentication;
public record RefreshTokenRequest(string AccessToken, string RefreshToken);