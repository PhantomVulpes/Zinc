using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Queries;

public record GetUserByKeyQuery(Guid UserKey) : Query<RegisteredUser>;
public class GetUserByKeyQueryHandler : QueryHandler<GetUserByKeyQuery, RegisteredUser>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public GetUserByKeyQueryHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task<RegisteredUser> InternalRequestAsync(GetUserByKeyQuery query)
    {
        var user = await userRepository.GetAsync(query.UserKey);
        return user;
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(GetUserByKeyQuery query) => AccessResult.Success().FromResult();

}