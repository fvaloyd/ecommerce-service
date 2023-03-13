namespace Ecommerce.Contracts.Endpoints;

public static class AuthEndpoints
{
    private const string root = "auth/";
    public const string Register = root+"register";
    public const string ConfirmEmail = root+"confirm-email";
    public const string RegisterAdmin = root+"register-admin";
    public const string Login = root+"login";
    public const string Logout = root+"logout"; 
}