using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Api.Responses;

public record LoginResponse
{
    public static LoginResponse Empty { get; } = new();

    public string Token { get; init; } = string.Empty;
    public string UserKey { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public Role Role { get; init; } = Role.Unknown;

    public static LoginResponse FromRegisteredUser(RegisteredUser user, string token) => Empty with
    {
        Token = token,
        UserKey = user.Key.ToString(),
        FirstName = user.FirstName,
        LastName = user.LastName,
        Username = user.Username,
        Role = user.Role
    };
}