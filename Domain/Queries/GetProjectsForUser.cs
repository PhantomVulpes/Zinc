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

    protected override async Task<IEnumerable<Project>> InternalRequestAsync(GetProjectsForUser query) => await RetrieveProjects(queryProvider, query.UserKey);

    public static async Task<IEnumerable<Project>> RetrieveProjects(IQueryProvider<Project> queryProvider, Guid userKey)
    {
        var projects = (await queryProvider.BeginQueryAsync()).Where(project => project.CreatorKey == userKey || project.AllowedUserKeys.Contains(userKey)).OrderBy(project => project.Name);
        return projects;
    }
}
