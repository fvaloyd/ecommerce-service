using Ecommerce.Core.Models;

namespace Ecommerce.Core.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(MailRequest mailRequest);
    string GetMailTemplate<T>(string mailTemplateName, T mailTemplateModel) where T : class;
}