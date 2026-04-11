using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record RegisterNewUserRequest(string FirstName, string LastName, string Username, string PasswordRaw)
{
    public RegisterNewUserCommand ToCommand() => new(Guid.NewGuid(), FirstName, LastName, Username, PasswordRaw);
}
