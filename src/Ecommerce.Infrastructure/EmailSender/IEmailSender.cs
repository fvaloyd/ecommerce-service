using Ecommerce.Core.Models;

namespace Ecommerce.Infrastructure.EmailSender;

public interface IEmailSender
{
    Task<bool> SendAsync(MailRequest mailRequest);
    string GetCompiledTemplateAsync<T>(int templateId, T mailModel) where T : class;
}