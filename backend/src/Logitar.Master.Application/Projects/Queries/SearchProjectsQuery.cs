using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

internal record SearchProjectsQuery(SearchProjectsPayload payload) : IRequest<SearchResults<Project>>;
