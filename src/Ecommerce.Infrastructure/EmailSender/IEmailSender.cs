using Ecommerce.Infrastructure.EmailSender.Models;

namespace Ecommerce.Infrastructure.EmailSender;

public interface IEmailSender
{
    Task<bool> SendAsync(MailRequest mailRequest);
    Task<string> GetCompiledTemplateAsync<T>(string template, T templateModel) where T : class;
    Task<string> GetTemplate(int templateId);
}