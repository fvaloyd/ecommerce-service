using Ecommerce.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;

namespace Ecommerce.Infrastructure.EmailSender;

public class SendiblueService : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;

    public SendiblueService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task<bool> SendAsync(MailRequest mailRequest)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
        mailMessage.To.Add(new MailboxAddress("", mailRequest.Email));

        mailMessage.Subject = mailRequest.Subject;
        mailMessage.Body = new TextPart("html") { Text = mailRequest.Body };

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_smtpOptions.Server, Convert.ToInt32(_smtpOptions.Port), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_smtpOptions.SenderEmail, _smtpOptions.Password);
        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
        return true;
    }

    public async Task<string> GetCompiledTemplateAsync<T>(string template, T mailModel) where T : class
    {
        IRazorEngine razorEngine = new RazorEngine();
        IRazorEngineCompiledTemplate modifiedMailTemplate = await razorEngine.CompileAsync(template);
        return await modifiedMailTemplate.RunAsync(model: mailModel);
    }

    public async Task<string> GetTemplate(int templateId)
    {
        var sendiblueApi = new sib_api_v3_sdk.Api.TransactionalEmailsApi();
        sib_api_v3_sdk.Model.GetSmtpTemplateOverview result = await sendiblueApi.GetSmtpTemplateAsync((long)templateId);
        string template = result.HtmlContent.Replace("@media", "@@ media");
        return template;
    }
}