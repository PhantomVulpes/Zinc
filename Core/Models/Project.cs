using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Zinc.Core.Logging;

namespace Vulpes.Zinc.Core.Models;

public record Project : AggregateRoot
{
    public static Project Empty => new();
    public static Project Default => Empty with
    {
        Key = Guid.NewGuid(),
        DefaultTicketStatus = TicketStatus.InReview,
        Status = ProjectStatus.Open,
    };

    public string Name { get; init; } = string.Empty;
    public string Shorthand { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public TicketStatus DefaultTicketStatus { get; init; } = TicketStatus.Unknown;

    public IEnumerable<Guid> AllowedUserKeys { get; init; } = [];
    public Guid CreatorKey { get; init; } = Guid.Empty;
    public ProjectStatus Status { get; init; } = ProjectStatus.Unknown;

    public IEnumerable<string> Labels { get; init; } = [];

    public AccessResult UserIsAllowed(Guid userKey) => CreatorKey == userKey || AllowedUserKeys.Contains(userKey) ? AccessResult.Success() : AccessResult.Fail($"User {userKey} is not allowed to access project {Name}.");
    public AccessResult UserIsAllowed(RegisteredUser user) => UserIsAllowed(user.Key);

    public AggregateRootValidationModel<Project> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."))
            .InvalidIf(() => string.IsNullOrEmpty(Name), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Name)} is required."))
            .InvalidIf(() => string.IsNullOrEmpty(Shorthand), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Shorthand)} is required."))
            ;

        return new AggregateRootValidationModel<Project>(this, validationBuilder);
    }
}

public enum ProjectStatus
{
    Unknown,
    Open,
    Closed,
    Archived,
}