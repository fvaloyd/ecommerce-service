using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Payment;

using Stripe;

namespace Ecommerce.Api.IntegrationTests.Startup;

public class StripeServiceMock : IStripeService
{
    public Task<Card> CreateCard(string validatedCardToken, string customerId)
    {
        return Task.FromResult(new Card
        {
            Id = "test123"
        });
    }

    public Token CreateCardToken(CardOptions card)
    {
        return new Token{
            Id = "test123"
        };
    }

    public Charge CreateChargeToken(string cardToken, decimal amount, ApplicationUser user)
    {
        return new Charge{
            CustomerId = user.CustomerId,
            Amount = (long)amount,
            BalanceTransactionId = "test123",
            Status = "succeeded"
        };
    }

    public Task<Customer> CreateCustomerToken(ApplicationUser user)
    {
        return Task.FromResult(new Customer{
            Id = "test123",
            Created = DateTime.Now,
            Description = "test",
            Email = user.Email,
            Name = user.UserName,
            Phone = user.PhoneNumber,
        });
    }

    public Task<Refund> CreateRefundToken(string chargeToken)
    {
        return Task.FromResult(new Refund{
            Id = "test123"
        });
    }

    public void DeleteCustomer(ApplicationUser user)
    {
    }
}