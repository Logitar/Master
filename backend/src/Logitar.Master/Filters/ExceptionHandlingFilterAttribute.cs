using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Master.Filters;

internal class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    else if (context.Exception is InvalidCredentialsException)
    {
      context.Result = new BadRequestObjectResult(new Error(code: "InvalidCredentials", InvalidCredentialsException.ErrorMessage));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }

  private static BadRequestObjectResult HandleValidationException(ExceptionContext context)
  {
    ValidationException exception = (ValidationException)context.Exception;
    return new BadRequestObjectResult(new { exception.Errors });
  }
}
