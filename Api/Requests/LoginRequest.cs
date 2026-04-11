using Vulpes.Zinc.Core.Queries;

namespace Vulpes.Zinc.Api.Requests;

public record LoginRequest(string Username, string PasswordRaw)
{
    public GetUserByLoginCredentialsQuery ToQuery() => new(Username, PasswordRaw);
}