using Logitar.Net.Http;

namespace Logitar.Master.Extensions;

internal static class HttpContextExtensions
{
  public static Uri GetBaseUri(this HttpContext context)
  {
    string host = context.Request.Host.Value;
    int index = host.IndexOf(':');

    IUrlBuilder builder = new UrlBuilder().SetScheme(context.Request.Scheme, inferPort: true);
    if (index < 0)
    {
      builder.SetHost(host);
    }
    else
    {
      builder.SetHost(host[..index]).SetPort(ushort.Parse(host[(index + 1)..]));
    }
    return builder.BuildUri();
  }
  public static Uri BuildLocation(this HttpContext context, string path, IEnumerable<KeyValuePair<string, string>> parameters)
  {
    UrlBuilder builder = new(context.GetBaseUri());
    builder.SetPath(path);
    foreach (KeyValuePair<string, string> parameter in parameters)
    {
      builder.SetParameter(parameter.Key, parameter.Value);
    }
    return builder.BuildUri();
  }
}
