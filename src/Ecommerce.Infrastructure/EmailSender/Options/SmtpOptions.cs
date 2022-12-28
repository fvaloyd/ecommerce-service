namespace Ecommerce.Infrastructure.EmailSender.Options;

public class SmtpOptions
{
    public string ApiKey { get; set; } = null!;
    public string Server { get; set; } = null!;
    public string Port { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}