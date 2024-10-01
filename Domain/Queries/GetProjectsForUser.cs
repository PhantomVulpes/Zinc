using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetProjectsForUser(Guid UserKey) : Query;
public class GetProjectsForUserHandler : QueryHandler<GetProjectsForUser, IEnumerable<Project>>
{
    private readonly IQueryProvider<Project> queryProvider;

    public GetProjectsForUserHandler(IQueryProvider<Project> queryProvider)
    {
        this.queryProvider = queryProvider;
    }

    protected override async Task<IEnumerable<Project>> InternalRequestAsync(GetProjectsForUser query)
    {
        var projects = (await queryProvider.BeginQueryAsync()).Where(project => project.CreatorKey == query.UserKey || project.AllowedUserKeys.Contains(query.UserKey));
        return projects;
    }
}
