using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.EmailSender;

public record EmailConfirmationMailModel (ApplicationUser User, string ConfirmationLink);
