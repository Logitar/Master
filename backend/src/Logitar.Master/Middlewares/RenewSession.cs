using Logitar.Master.Constants;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.Web.Extensions;

namespace Logitar.Master.Middlewares;

internal class RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
  {
    if (context.GetSessionId() == null)
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken != null)
      {
        try
        {
          RenewPayload payload = new()
          {
            RefreshToken = refreshToken
          };
          Session session = await sessionService.RenewAsync(payload);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}
