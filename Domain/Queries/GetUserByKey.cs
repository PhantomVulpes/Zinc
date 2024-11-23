using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetUserByKey(Guid UserKey) : Query;
public class GetUserByKeyHandler : QueryHandler<GetUserByKey, ZincUser>
{
    private readonly IDataRepository<ZincUser> userRepository;

    public GetUserByKeyHandler(IDataRepository<ZincUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task<ZincUser> InternalRequestAsync(GetUserByKey query) => await userRepository.GetAsync(query.UserKey);
}
