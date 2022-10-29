using Ecommerce.Core.Models;

namespace Ecommerce.Infrastructure.EmailSender;

public interface IEmailSender
{
    Task<bool> SendAsync(MailRequest mailRequest);
    string GetTemplate<T>(string mailTemplateName, T mailTemplateModel) where T : class;
}