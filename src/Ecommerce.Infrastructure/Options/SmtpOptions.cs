namespace Ecommerce.Infrastructure.Options;

public class SmtpOptions
{
    public string Server { get; set; } = null!;
    public string Port { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}