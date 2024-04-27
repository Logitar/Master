using Logitar.Master.Application.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class MessageService : IMessageService
{
  private readonly IMessageClient _messageClient;

  public MessageService(IMessageClient messageClient)
  {
    _messageClient = messageClient;
  }

  public async Task<SentMessages> SendAsync(string template, Email email, string? locale, Dictionary<string, string>? variables, CancellationToken cancellationToken)
  {
    RecipientPayload recipient = new()
    {
      Type = RecipientType.To,
      Address = email.Address
    };
    return await SendAsync(template, recipient, locale, variables, cancellationToken);
  }

  public async Task<SentMessages> SendAsync(string template, User user, string? locale, Dictionary<string, string>? variables, CancellationToken cancellationToken)
  {
    RecipientPayload recipient = new()
    {
      Type = RecipientType.To,
      UserId = user.Id
    };
    return await SendAsync(template, recipient, locale, variables, cancellationToken);
  }

  private async Task<SentMessages> SendAsync(string template, RecipientPayload recipient, string? locale, IEnumerable<KeyValuePair<string, string>>? variables, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = new(template)
    {
      Locale = locale
    };
    payload.Recipients.Add(recipient);
    if (variables != null)
    {
      foreach (KeyValuePair<string, string> variable in variables)
      {
        payload.Variables.Add(new Variable(variable));
      }
    }
    RequestContext context = new(cancellationToken); // TOOD(fpion): send user
    return await _messageClient.SendAsync(payload, context);
  }
}
