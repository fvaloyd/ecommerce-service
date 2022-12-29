using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Payment.Models;
using Stripe;

namespace Ecommerce.Infrastructure.Payment;

public interface IStripeService
{
    Token CreateCardToken(PayRequest card);
    Task<Card> CreateCard(string validatedCardToken, string customerId);
    Charge CreateChargeToken(string cardToken, decimal amount, ApplicationUser user);
    Task<Refund> CreateRefundToken(string chargeToken);
    Task<Customer> CreateCustomerToken(ApplicationUser user);
    void DeleteCustomer(ApplicationUser user);
}
