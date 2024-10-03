using Vulpes.Electrum.Core.Domain.Security;

namespace Vulpes.Zinc.Domain.Models;
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

    protected override ElectrumValidationResult InternalValidate()
    {
        var internalResult = base.InternalValidate();
        return ElectrumValidationResult.Verify(() => Shorthand.Length == 4, "Shorthand token must be exactly 4 characters long.",
            ElectrumValidationResult.Verify(() => !string.IsNullOrEmpty(Shorthand), "Shorthand token is required", internalResult));
    }
}

public enum ProjectStatus
{
    Unknown,
    Open,
    Closed,
    Archived,
}