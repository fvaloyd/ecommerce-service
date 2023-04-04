namespace Ecommerce.Contracts.Responses;

public record AuthenticateResponse(string AccessToken, string RefreshToken);