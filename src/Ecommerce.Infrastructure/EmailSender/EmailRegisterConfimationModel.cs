using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.EmailSender;

public record EmailConfirmationModel (ApplicationUser User, string ConfirmationLink);
