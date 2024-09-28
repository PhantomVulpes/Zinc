using Vulpes.Zinc.Domain.Security;

namespace Vulpes.Zinc.Domain.Models;
public record User : AggregateRoot
{
    public static User Empty => new();
    public static User Default { get; } = Empty with
    {
        Key = Guid.NewGuid(),
        Role = Role.Basic
    };

    public string Username { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public Role Role { get; init; } = Role.Unknown;
}
