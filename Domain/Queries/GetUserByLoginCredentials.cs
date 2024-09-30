using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;

public record GetUserByLoginCredentials(string UsernameOrEmail, string Password) : Query();
public class GetUserByLoginCredentialsHandler : QueryHandler<GetUserByLoginCredentials, ZincUser>
{
    private readonly IQueryProvider<ZincUser> queryProvider;
    private readonly IKnoxHasher knoxHasher;

    private readonly IDataRepository<ZincUser> dataRepository;

    public GetUserByLoginCredentialsHandler(IQueryProvider<ZincUser> queryProvider, IKnoxHasher knoxHasher, IDataRepository<ZincUser> dataRepository)
    {
        this.queryProvider = queryProvider;
        this.knoxHasher = knoxHasher;
        this.dataRepository = dataRepository;
    }

    protected override async Task<ZincUser> InternalRequestAsync(GetUserByLoginCredentials query)
    {
        var d = await dataRepository.GetAsync(Guid.Parse("6d05a1be-4431-4d2a-84d4-9112afe566b1"));
        var allQuery = (await queryProvider.BeginQueryAsync()).ToList();
        var userRelic = (await queryProvider.BeginQueryAsync())
            .Where(user => user.Username.ToLower() == query.UsernameOrEmail.ToLower()/* || user.Email.ToLower() == query.UsernameOrEmail.ToLower()*/)
            .ConcealFirst()
            ;

        // TODO: Apparently IsWorthless doesn't work right. Who wrote that unit test!?
        // ThrowIfInvalidCredentials(!userRelic.IsWorthless);

        var user = userRelic.RevealOrHoax();
        var passwordCorrect = knoxHasher.CompareHash(user.PasswordHash, query.Password);

        ThrowIfInvalidCredentials(passwordCorrect);

        // TODO: Also apparently I don't have a .Conceal extension for Relics either. This is madness.
        return user;
    }

    /// <summary>
    /// Check and throw an exception if it is invalid.
    /// </summary>
    /// <param name="isValid">The positive value to check if a user's credential is valid.</param>
    private static void ThrowIfInvalidCredentials(bool isValid)
    {
        if (!isValid)
        {
            throw new NullReferenceException("Username or password is not valid.");
        }
    }
}
