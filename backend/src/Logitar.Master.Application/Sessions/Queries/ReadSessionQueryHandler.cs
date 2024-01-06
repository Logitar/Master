using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Queries;

internal class ReadSessionQueryHandler : IRequestHandler<ReadSessionQuery, Session?>
{
  private readonly ISessionQuerier _sessionQuerier;

  public ReadSessionQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<Session?> Handle(ReadSessionQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
