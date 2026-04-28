using Vulpes.Electrum.Domain.Models;
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
    public IEnumerable<Ticket> Tickets { get; init; } = [];

    public IEnumerable<string> Labels { get; init; } = [];

    public AggregateRootValidationModel<Project> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."))
            .InvalidIf(() => string.IsNullOrEmpty(Name), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Name)} is required."))
            .InvalidIf(() => string.IsNullOrEmpty(Shorthand), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Shorthand)} is required."))
            .InvalidIf(() => !AllowedUserKeys.Any(), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(AllowedUserKeys)} must contain at least one user."))
            ;

        return new AggregateRootValidationModel<Project>(this, validationBuilder);
    }

    public SaveModel<Project> PrepareForSave()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        }).Validate();

        return new(validatedObject, EditingToken);
    }

    public InsertModel<Project> PrepareForInsert()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        }).Validate();

        return new(validatedObject);
    }

    public override string ToLogName() => $"{Name} ({Key})";

}

public enum ProjectStatus
{
    Unknown,
    Open,
    Closed,
    Archived,
}