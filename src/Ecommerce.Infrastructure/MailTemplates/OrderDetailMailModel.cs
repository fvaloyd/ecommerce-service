#nullable disable
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.MailTemplates;

public record OrderDetailMailModel (ApplicationUser User, IEnumerable<OrderDetail> OrderDetails);