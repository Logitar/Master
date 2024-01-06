using Logitar.EventSourcing;
using Logitar.Master.Application;
using Logitar.Master.Contracts.Users;
using Logitar.Master.Web.Extensions;

namespace Logitar.Master;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"The {nameof(_httpContextAccessor.HttpContext)} is required.");

  public ActorId ActorId
  {
    get
    {
      User? user = Context.GetUser();
      if (user != null)
      {
        return new(user.Id);
      }

      return default;
    }
  }

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}
