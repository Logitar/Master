using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Queries;

internal record ReadSessionQuery(string Id) : IRequest<Session?>;
