using Logitar.Portal.Client;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Master.PortalSeeding.Worker;

internal record WorkerPortalSettings : IPortalSettings
{
  private static WorkerPortalSettings? _instance = null;
  public static WorkerPortalSettings Instance => _instance ?? throw new InvalidOperationException($"The settings have not been initialized. You must call the '{nameof(Initialize)}' method once.");

  public static IPortalSettings Initialize(IPortalSettings portalSettings)
  {
    if (_instance != null)
    {
      throw new InvalidOperationException($"The settings have already been initialized. You may only call the '{nameof(Initialize)}' method once.");
    }

    _instance = new(portalSettings);

    return _instance;
  }

  public string? ApiKey { get; }
  public BasicCredentials? Basic { get; }
  public string? BaseUrl { get; }
  public string? Realm { get; private set; }

  private WorkerPortalSettings(IPortalSettings portalSettings)
  {
    ApiKey = portalSettings.ApiKey;
    Basic = portalSettings.Basic;
    BaseUrl = portalSettings.BaseUrl;
  }

  public void SetRealm(Realm realm)
  {
    Realm = realm.Id.ToString();
  }
}
