namespace Ecommerce.Api.Dtos.Authentication;
public record AuthenticateResponse(string AccessToken, string RefreshToken);