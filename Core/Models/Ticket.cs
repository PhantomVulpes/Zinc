using Vulpes.Electrum.Domain.Validation;
using Vulpes.Zinc.Core.Logging;
using Vulpes.Zinc.Core.Validation;

namespace Vulpes.Zinc.Core.Models;

public record Ticket
{
    public static Ticket Empty => new();
    public static Ticket Default(int index) =>
        Empty with
        {
            Index = index,
            CreatedDate = DateTime.UtcNow,
        };

    public int Index { get; init; } = int.MinValue;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid AssignedToKey { get; init; } = Guid.Empty;
    public Guid ReporterKey { get; init; } = Guid.Empty;
    public IEnumerable<Comment> Comments { get; init; } = [];
    public DateTime CreatedDate { get; init; } = DateTime.MinValue;
    public DateTime CompletedDate { get; init; } = DateTime.MinValue;
    public IEnumerable<string> Labels { get; init; } = [];

    public TicketStatus Status { get; init; } = TicketStatus.Unknown;

    public Ticket AddComment(Comment comment)
    {
        var comments = Comments.Append(comment);
        return this with
        {
            Comments = comments,
        };
    }

    public IValidationModel<Ticket> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Index <= 0, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Index)} cannot be below 1."))
            .InvalidIf(() => string.IsNullOrEmpty(Title), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Title)} is required."))
            .InvalidIf(() => ReporterKey == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(ReporterKey)} is required."))
            .InvalidIf(() => Status == TicketStatus.Unknown, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Status)} is required."))
            ;

        return new GenericValidationModel<Ticket>(this, validationBuilder);
    }
}

public enum TicketStatus
{
    Unknown,
    InReview,
    Open,
    InProgress,
    Complete,
    Cancelled
}