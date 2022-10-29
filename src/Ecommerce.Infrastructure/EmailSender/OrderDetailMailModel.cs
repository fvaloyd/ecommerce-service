#nullable disable
using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.EmailSender;

public record OrderDetailMailModel (ApplicationUser User, IEnumerable<OrderDetail> OrderDetails);