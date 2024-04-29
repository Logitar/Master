﻿using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Master.Application.Accounts;

internal class InvalidOneTimePasswordPurpose : BadRequestException
{
  private const string ErrorMessage = "The specified purpose did not match the expected One-Time Passord (OTP) purpose.";

  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
  }
  public string ExpectedPurpose
  {
    get => (string)Data[nameof(ExpectedPurpose)]!;
    private set => Data[nameof(ExpectedPurpose)] = value;
  }
  public string? ActualPurpose
  {
    get => (string?)Data[nameof(ActualPurpose)];
    private set => Data[nameof(ActualPurpose)] = value;
  }

  public override Error Error => new(code: "InvalidCredentials", message: "The specified credentials did not match.");

  public InvalidOneTimePasswordPurpose(OneTimePassword oneTimePassword, string purpose) : base(BuildMessage(oneTimePassword, purpose))
  {
    OneTimePasswordId = oneTimePassword.Id;
    ExpectedPurpose = purpose;
    ActualPurpose = oneTimePassword.TryGetPurpose();
  }

  private static string BuildMessage(OneTimePassword oneTimePassword, string purpose) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(OneTimePasswordId), oneTimePassword.Id)
    .AddData(nameof(ExpectedPurpose), purpose)
    .AddData(nameof(ActualPurpose), oneTimePassword.TryGetPurpose(), "<null>")
    .Build();
}
