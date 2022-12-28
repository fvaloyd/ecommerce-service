using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Stripe;

namespace Ecommerce.Infrastructure.Payment;

public interface IStripeService
{
    Token CreateCardToken(CardOptions card);
    Task<Card> CreateCard(string validatedCardToken, string customerId);
    Charge CreateChargeToken(string cardToken, decimal amount, ApplicationUser user);
    Task<Refund> CreateRefundToken(string chargeToken);
    Task<Customer> CreateCustomerToken(ApplicationUser user);
    void DeleteCustomer(ApplicationUser user);
}
