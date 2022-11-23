using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Api.Filters;

public class StripeExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is Stripe.StripeException stripeException)
        {
            var details = new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Stripe Exception",
                Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
                Detail = stripeException.Message,
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}