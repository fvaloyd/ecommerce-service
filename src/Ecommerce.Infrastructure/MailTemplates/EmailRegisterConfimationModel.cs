using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.MailTemplates;

public record EmailRegisterConfirmationModel (ApplicationUser User, string ConfirmationLink);
