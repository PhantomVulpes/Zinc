using System.Security.Authentication;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Queries;

public record GetUserByLoginCredentialsQuery(string Username, string PasswordRaw) : Query<RegisteredUser>;
public class GetUserByLoginCredentialsQueryHandler : QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>
{
    private readonly IQueryProvider<RegisteredUser> queryProvider;
    private readonly IKnoxHasher knoxHasher;

    private readonly static string failureMessage = "Provided name or password was incorrect. Try again.";

    public GetUserByLoginCredentialsQueryHandler(IQueryProvider<RegisteredUser> queryProvider, IKnoxHasher knoxHasher)
    {
        this.queryProvider = queryProvider;
        this.knoxHasher = knoxHasher;
    }

    protected override async Task<RegisteredUser> InternalRequestAsync(GetUserByLoginCredentialsQuery query)
    {
        var user = (await queryProvider.BeginQueryAsync())
            .SingleOrDefault(user =>
                user.Username.Equals(query.Username, StringComparison.CurrentCultureIgnoreCase))
                ?? throw new InvalidCredentialException(failureMessage)
            ;

        var passwordMatch = knoxHasher.CompareHash(user.PasswordHash, query.PasswordRaw);

        if (!passwordMatch)
        {
            throw new InvalidCredentialException(failureMessage);
        }

        return user;
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(GetUserByLoginCredentialsQuery query) => AccessResult.Success().FromResult();
}