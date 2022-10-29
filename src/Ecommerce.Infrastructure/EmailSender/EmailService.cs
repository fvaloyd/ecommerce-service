using System.Text;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RazorEngineCore;

namespace Ecommerce.Infrastructure.EmailSender;

public class EmailService : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;

    public EmailService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task<bool> SendAsync(MailRequest mailRequest)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
        mailMessage.To.Add(new MailboxAddress("", mailRequest.Email));

        mailMessage.Subject = mailRequest.Subject;
        mailMessage.Body = new TextPart("html"){Text = mailRequest.Body};

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(_smtpOptions.Server, Convert.ToInt32(_smtpOptions.Port), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_smtpOptions.SenderEmail, _smtpOptions.Password);
        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
        return true;
    }

    public string GetTemplate<T>(string mailTemplateName, T mailTemplateModel) where T : class
    {
        string mailTemplate = LoadTemplate(mailTemplateName);

        IRazorEngine razorEngine = new RazorEngine();
        IRazorEngineCompiledTemplate modifiedMailTemplate = razorEngine.Compile(mailTemplate);

        return modifiedMailTemplate.Run(model: mailTemplateModel);
    }

    private string LoadTemplate(string mailTemplateName)
    {
        string incorrectBaseDir = Environment.CurrentDirectory;
        string basedir = incorrectBaseDir.Replace("Ecommerce.Api", "Ecommerce.Infrastructure");
        string templateDir = Path.Combine(basedir, "MailTemplates");
        string templatePath = Path.Combine(templateDir, $"{mailTemplateName}.cshtml");

        using FileStream fileStream = new(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader streamReader = new(fileStream, Encoding.Default);

        string mailTemplate = streamReader.ReadToEnd();
        streamReader.Close();

        return mailTemplate;
    }
}