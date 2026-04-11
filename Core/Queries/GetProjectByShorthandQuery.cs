using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Queries;

public record GetProjectByShorthandQuery(string ProjectShorthand, Guid UserKey) : Query<Project>;
public class GetProjectByShorthandQueryHandler : QueryHandler<GetProjectByShorthandQuery, Project>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IQueryProvider<Project> projectQueryProvider;

    public GetProjectByShorthandQueryHandler(IModelRepository<RegisteredUser> userRepository, IQueryProvider<Project> projectQueryProvider)
    {
        this.userRepository = userRepository;
        this.projectQueryProvider = projectQueryProvider;
    }

    protected override async Task<Project> InternalRequestAsync(GetProjectByShorthandQuery query)
    {
        var project = await GetProjectAsync(query.ProjectShorthand);
        return project;
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(GetProjectByShorthandQuery query)
    {
        var user = await userRepository.GetAsync(query.UserKey);

        if (user.Role == Role.Admin)
        {
            return AccessResult.Success();
        }

        var project = await GetProjectAsync(query.ProjectShorthand);

        if (!project.AllowedUserKeys.Contains(query.UserKey))
        {
            return AccessResult.Fail($"User {user.ToLogName()} does not have access to {project.ToLogName()}.");
        }

        return AccessResult.Success();
    }

    private async Task<Project> GetProjectAsync(string projectShorthand)
    {
        var project = (await projectQueryProvider.BeginQueryAsync()).Single(project => project.Shorthand == projectShorthand);
        return project;
    }
}