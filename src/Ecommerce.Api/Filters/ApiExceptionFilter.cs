using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Stripe;

namespace Ecommerce.Api.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionsHandlers;
    public ApiExceptionFilter()
    {
        _exceptionsHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            {typeof(ArgumentException), HandleArgumentException},
            {typeof(ArgumentNullException), HandleArgumentNullException},
            {typeof(InvalidOperationException), HandleInvalidOperationException},
            {typeof(StripeException), HandleStripeException},
        };
    }
    public void OnException(ExceptionContext context)
    {
        HandleException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionsHandlers.ContainsKey(type))
        {
            _exceptionsHandlers[type].Invoke(context);
            return;
        }
    }

    private void HandleStripeException(ExceptionContext context)
    {
        var exception = (StripeException)context.Exception;

        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Stripe Exception",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Detail = exception.Message,
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleArgumentException(ExceptionContext context)
    {
        var exception = (ArgumentException)context.Exception;

        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Title = "Argument exception",
            Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleArgumentNullException(ExceptionContext context)
    {
        var exception = (ArgumentNullException)context.Exception;

        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Title = "Null argument exception",
            Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    private void HandleInvalidOperationException(ExceptionContext context)
    {
        var exception = (InvalidOperationException)context.Exception;

        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Title = "Invalid operation exception",
            Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }
}
