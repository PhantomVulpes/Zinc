using System;
using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Zinc.Core.Logging;

namespace Vulpes.Zinc.Core.Models;

public record Ticket : AggregateRoot
{
    public static Ticket Empty => new();
    public static Ticket Default =>
        Empty with
        {
            Key = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
        };

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid AssignedToKey { get; init; } = Guid.Empty;
    public Guid ReporterKey { get; init; } = Guid.Empty;
    public IEnumerable<Comment> Comments { get; init; } = [];
    public DateTime CreatedDate { get; init; } = DateTime.MinValue;
    public DateTime CompletedDate { get; init; } = DateTime.MinValue;
    public Guid ProjectKey { get; init; } = Guid.Empty;
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

    public AggregateRootValidationModel<Ticket> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."))
            .InvalidIf(() => string.IsNullOrEmpty(Title), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Title)} is required."))
            .InvalidIf(() => ReporterKey == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(ReporterKey)} is required."))
            .InvalidIf(() => ProjectKey == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(ProjectKey)} is required."))
            .InvalidIf(() => Status == TicketStatus.Unknown, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Status)} is required."))
            ;

        return new AggregateRootValidationModel<Ticket>(this, validationBuilder);
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