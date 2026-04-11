using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Queries;

public record GetProjectByKeyQuery(Guid ProjectKey, Guid UserKey) : Query<Project>;
public class GetProjectByKeyQueryHandler : QueryHandler<GetProjectByKeyQuery, Project>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<Project> projectRepository;

    public GetProjectByKeyQueryHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<Project> projectRepository)
    {
        this.userRepository = userRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task<Project> InternalRequestAsync(GetProjectByKeyQuery query)
    {
        var project = await projectRepository.GetAsync(query.ProjectKey);
        return project;
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(GetProjectByKeyQuery query)
    {
        var user = await userRepository.GetAsync(query.UserKey);

        if (user.Role == Role.Admin)
        {
            return AccessResult.Success();
        }

        var project = await projectRepository.GetAsync(query.ProjectKey);

        if (!project.AllowedUserKeys.Contains(query.UserKey))
        {
            return AccessResult.Fail($"User {user.ToLogName()} does not have access to {project.ToLogName()}.");
        }

        return AccessResult.Success();
    }
}