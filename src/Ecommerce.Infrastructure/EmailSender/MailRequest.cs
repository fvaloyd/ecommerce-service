namespace Ecommerce.Core.Models;

public record MailRequest
(
    string Body,
    string Subject,
    string Email
);