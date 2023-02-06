using Ecommerce.Infrastructure.EmailSender;
using Ecommerce.Infrastructure.EmailSender.Common;
using Ecommerce.Infrastructure.EmailSender.Models;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.BackgroundJobs;

public class SendMailToConfirmEmail
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly LinkGenerator _linkGenerator;
    private readonly IEmailSender _emailSender;

    public SendMailToConfirmEmail(UserManager<ApplicationUser> userManager, LinkGenerator linkGenerator, IEmailSender emailSender)
    {
        _userManager = userManager;
        _linkGenerator = linkGenerator;
        _emailSender = emailSender;
    }

    public async Task Handle(string userId, string scheme, string hostValue)
    {
        var host = new HostString(hostValue);
        var user = await _userManager.FindByIdAsync(userId);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);

        var confirmationLink = _linkGenerator.GetUriByAction(
            action: "ConfirmEmail",
            controller: "Auth",
            values: new { token, email = user!.Email },
            scheme: scheme,
            host: host
        );

        var mail = await CreateMail(user!, confirmationLink!);

        await _emailSender.SendAsync(mail);
    }

    private async Task<MailRequest> CreateMail(ApplicationUser user, string confirmationLink)
    {
        var emailConfirmationMailModel = new EmailConfirmationMailModel(User: user, ConfirmationLink: confirmationLink);

        string template = await _emailSender.GetTemplate(((int)MailTemplates.EmailConfirmation));

        string compiledTemplate = await _emailSender.GetCompiledTemplateAsync(template, emailConfirmationMailModel);

        return new MailRequest(
            Body: compiledTemplate,
            Subject: "Email confirmation",
            Email: user.Email!
        );
    }
}
