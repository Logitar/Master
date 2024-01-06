using FluentValidation;
using Logitar.Identity.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Master.Filters;

internal class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
  private readonly Dictionary<Type, Func<ExceptionContext, IActionResult>> _handlers = new()
  {
    [typeof(EmailAddressAlreadyUsedException)] = HandleEmailAddressAlreadyUsedException,
    [typeof(ValidationException)] = HandleValidationException
  };

  public override void OnException(ExceptionContext context)
  {
    if (_handlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, IActionResult>? handler))
    {
      context.Result = handler(context);
      context.ExceptionHandled = true;
    }
    //else if (context.Exception is AggregateNotFoundException)
    //{
    //}
    //else if (context.Exception is InvalidCredentialsException)
    //{
    //  context.Result = new BadRequestObjectResult(new Error(code: "InvalidCredentials", InvalidCredentialsException.ErrorMessage));
    //  context.ExceptionHandled = true;
    //}
    //else if (context.Exception is UniqueNameAlreadyUsedException)
    //{
    //}
    else
    {
      base.OnException(context);
    }
  }

  private static ConflictObjectResult HandleEmailAddressAlreadyUsedException(ExceptionContext context)
  {
    //EmailAddressAlreadyUsedException exception = (EmailAddressAlreadyUsedException)context.Exception;
    //Error error = new(exception.GetErrorCode(), EmailAddressAlreadyUsedException.ErrorMessage)
    //{
    //  Data =
    //  [
    //    new ErrorData(nameof(exception.TenantId), exception.TenantId),
    //    new ErrorData(nameof(exception.EmailAddress), exception.EmailAddress)
    //  ]
    //};
    //return new(error);
    throw new NotImplementedException(); // TODO(fpion): error object
  }

  private static BadRequestObjectResult HandleValidationException(ExceptionContext context)
  {
    //ValidationException exception = (ValidationException)context.Exception;
    //Error error = new(exception.GetErrorCode(), exception.Message)
    //{
    //  Data = [new ErrorData(nameof(exception.Errors), exception.Errors)]
    //};
    //return new(error);
    throw new NotImplementedException(); // TODO(fpion): error object
  }
}
