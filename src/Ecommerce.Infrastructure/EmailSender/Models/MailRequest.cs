namespace Ecommerce.Infrastructure.EmailSender.Models;
public record MailRequest
(
    string Body,
    string Subject,
    string Email
);