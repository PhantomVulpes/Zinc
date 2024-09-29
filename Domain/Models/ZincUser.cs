using Vulpes.Zinc.Domain.Security;

namespace Vulpes.Zinc.Domain.Models;
public record ZincUser : AggregateRoot
{
    public static ZincUser Empty => new();
    public static ZincUser Default { get; } = Empty with
    {
        Key = Guid.NewGuid(),
        Role = Role.Basic
    };

    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public Role Role { get; init; } = Role.Unknown;
}
