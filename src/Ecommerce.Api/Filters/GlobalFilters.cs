using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Api.Filters;

public class GlobalFilters : IExceptionFilter
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionsHandlers;
    public GlobalFilters()
    {
        _exceptionsHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            {typeof(ArgumentException), HandleArgumentException},
            {typeof(ArgumentNullException), HandleArgumentNullException},
            {typeof(InvalidOperationException), HandleInvalidOperationException},
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
