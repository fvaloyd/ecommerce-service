namespace Ecommerce.Api.Dtos.Authentication;
public record RefreshTokenRequest(string AccessToken, string RefreshToken);