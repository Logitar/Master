using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logitar.Master.Filters;

internal class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is InvalidCredentialsException)
    {
      context.Result = new BadRequestObjectResult(new Error(code: "InvalidCredentials", InvalidCredentialsException.ErrorMessage));
      context.ExceptionHandled = true;
    }
    else
    {
      base.OnException(context);
    }
  }
}
