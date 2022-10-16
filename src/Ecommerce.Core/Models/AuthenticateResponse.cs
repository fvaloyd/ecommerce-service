namespace Ecommerce.Core.Models;

public record AuthenticateResponse(string AccessToken, string RefreshToken);