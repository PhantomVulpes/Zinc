using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Queries;

public record GetAllAccessibleProjectsQuery(Guid UserKey) : Query<IQueryable<Project>>;
public class GetAllAccessibleProjectsQueryHandler : QueryHandler<GetAllAccessibleProjectsQuery, IQueryable<Project>>
{
    private readonly IQueryProvider<Project> projectQueryProvider;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public GetAllAccessibleProjectsQueryHandler(IQueryProvider<Project> projectQueryProvider, IModelRepository<RegisteredUser> userRepository)
    {
        this.projectQueryProvider = projectQueryProvider;
        this.userRepository = userRepository;
    }

    protected override async Task<IQueryable<Project>> InternalRequestAsync(GetAllAccessibleProjectsQuery query)
    {
        var queryResult = await projectQueryProvider.BeginQueryAsync();

        var user = await userRepository.GetAsync(query.UserKey);

        // Admins get everything.
        if (user.Role == Role.Admin)
        {
            return queryResult;
        }

        return queryResult.Where(project => project.AllowedUserKeys.Contains(query.UserKey));
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(GetAllAccessibleProjectsQuery query) => AccessResult.Success().FromResult();

}