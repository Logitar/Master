using Logitar.Master.Constants;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.Contracts.Users;
using Logitar.Master.Settings;

namespace Logitar.Master.Web.Extensions;

internal static class HttpContextExtensions
{
  private const string SessionIdKey = "SessionId";
  private const string SessionKey = "Session";
  private const string UserKey = "User";

  private static readonly Encoding Encoding = Encoding.UTF8;

  public static Session? GetSession(this HttpContext context) => context.GetItem<Session>(SessionKey);
  public static User? GetUser(this HttpContext context) => context.GetItem<User>(UserKey);
  private static T? GetItem<T>(this HttpContext context, object key)
  {
    return context.Items.TryGetValue(key, out object? value) ? (T?)value : default;
  }

  public static void SetSession(this HttpContext context, Session? session) => context.SetItem(SessionKey, session);
  public static void SetUser(this HttpContext context, User? user) => context.SetItem(UserKey, user);
  private static void SetItem<T>(this HttpContext context, object key, T? value)
  {
    if (value == null)
    {
      context.Items.Remove(key);
    }
    else
    {
      context.Items[key] = value;
    }
  }

  public static string? GetSessionId(this HttpContext context)
  {
    byte[]? bytes = context.Session.Get(SessionIdKey);

    return bytes == null ? null : Encoding.GetString(bytes);
  }
  public static bool IsSignedIn(this HttpContext context) => context.GetSessionId() != null;
  public static void SignIn(this HttpContext context, Session session)
  {
    context.Session.Set(SessionIdKey, Encoding.GetBytes(session.Id));

    if (session.RefreshToken != null)
    {
      CookiesSettings cookiesSettings = context.RequestServices.GetRequiredService<CookiesSettings>();
      CookieOptions options = new()
      {
        HttpOnly = cookiesSettings.RefreshToken.HttpOnly,
        MaxAge = cookiesSettings.RefreshToken.MaxAge,
        SameSite = cookiesSettings.RefreshToken.SameSite,
        Secure = cookiesSettings.RefreshToken.Secure
      };
      context.Response.Cookies.Append(Cookies.RefreshToken, session.RefreshToken, options);
    }

    context.SetUser(session.User);
  }
  public static void SignOut(this HttpContext context)
  {
    context.Session.Clear();

    context.Response.Cookies.Delete(Cookies.RefreshToken);
  }
}
