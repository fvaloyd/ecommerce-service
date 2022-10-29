using Ecommerce.Core.Models;

namespace Ecommerce.Infrastructure.EmailSender;

public interface IEmailService
{
    Task<bool> SendEmailAsync(MailRequest mailRequest);
    string GetMailTemplate<T>(string mailTemplateName, T mailTemplateModel) where T : class;
}