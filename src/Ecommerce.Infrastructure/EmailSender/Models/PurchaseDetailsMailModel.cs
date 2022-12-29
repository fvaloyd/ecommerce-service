using Ecommerce.Core.Entities;
using Ecommerce.Infrastructure.Identity;

namespace Ecommerce.Infrastructure.EmailSender.Models;

public record PurchaseDetailsMailModel(ApplicationUser User, IEnumerable<OrderDetail> OrderDetails);