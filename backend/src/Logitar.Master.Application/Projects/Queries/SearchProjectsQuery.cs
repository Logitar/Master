using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

public record SearchProjectsQuery(SearchProjectsPayload Payload) : IRequest<SearchResults<Project>>;
