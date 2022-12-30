using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.EmailSender.Models;

public record EmailConfirmationMailModel(ApplicationUser User, string ConfirmationLink);
