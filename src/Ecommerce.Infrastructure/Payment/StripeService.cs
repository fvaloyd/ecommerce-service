using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Payment.Models;
using Stripe;

namespace Ecommerce.Infrastructure.Payment;

public class StripeService : IStripeService
{
    public async Task<Card> CreateCard(string validatedCardToken, string customerId)
    {
        var cardOptions = new CardCreateOptions
        {
            Source = validatedCardToken
        };
        var cardService = new CardService();
        return await cardService.CreateAsync(parentId: customerId, options: cardOptions);
    }

    public Token CreateCardToken(PayRequest card)
    {
        var tokenOptions = new TokenCreateOptions()
        {
            Card = new TokenCardOptions()
            {
                Number = card.Number,
                Cvc = card.Cvc,
                ExpMonth = card.ExpMonth,
                ExpYear = card.ExpYear,
            }
        };

        var service = new TokenService();
        var token = service.Create(tokenOptions);

        return token;
    }

    public Charge CreateChargeToken(string cardToken, decimal amount, ApplicationUser user)
    {
        var chargeOptions = new ChargeCreateOptions()
        {
            Amount = Convert.ToInt32(amount * 100),
            Currency = "usd",
            Source = cardToken,
            Customer = user.CustomerId,
            Description = "Pay for products",
            ReceiptEmail = user.Email
        };

        var chargeService = new ChargeService();
        var chargeToken = chargeService.Create(chargeOptions);

        return chargeToken;
    }

    public async Task<Customer> CreateCustomerToken(ApplicationUser user)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Description = "Ecommerce customer",
            Name = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber,
        };
        var service = new CustomerService();
        return await service.CreateAsync(customerOptions);
    }

    public async Task<Refund> CreateRefundToken(string chargeToken)
    {
        var refundOptions = new RefundCreateOptions
        {
            Charge = chargeToken,
        };

        var service = new RefundService();
        return await service.CreateAsync(refundOptions);
    }

    public void DeleteCustomer(ApplicationUser user)
    {
        var customerService = new CustomerService();
        customerService.Delete(user.CustomerId);
    }
}