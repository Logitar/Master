﻿namespace Logitar.Master.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
}
