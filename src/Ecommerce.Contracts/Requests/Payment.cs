namespace Ecommerce.Contracts.Requests;

public record PayRequest(string Number, string ExpMonth, string ExpYear, string Cvc);