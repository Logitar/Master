using Logitar.Master.Contracts.Users;
using Logitar.Master.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Master.Authorization;

internal class SystemUserAuthorizationHandler : AuthorizationHandler<SystemUserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public SystemUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemUserAuthorizationRequirement requirement)
  {
    if (_httpContextAccessor.HttpContext != null)
    {
      User? user = _httpContextAccessor.HttpContext.GetUser();
      if (user == null)
      {
        context.Fail(new AuthorizationFailureReason(this, "An authenticated user is required."));
      }
      else if (user.TenantId != null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The authenticated user should not be part of a tenant."));
      }
      else
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
