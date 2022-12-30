namespace Ecommerce.Infrastructure.Payment.Models;
public record PayRequest(string Number, string ExpMonth, string ExpYear, string Cvc);