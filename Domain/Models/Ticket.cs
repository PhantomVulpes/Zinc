namespace Vulpes.Zinc.Domain.Models;
public record Ticket : AggregateRoot
{
    // TODO: Add convention for null serialization from database. Changed name of thing from database, now it breaks unless I make a new one.
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

    public Dictionary<TicketRelationship, Guid> TicketRelationships { get; init; } = [];

    public TicketStatus Status { get; init; } = TicketStatus.Unknown;

    public Ticket AddComment(Comment comment)
    {
        var comments = Comments.Append(comment);
        return this with
        {
            Comments = comments,
        };
    }

    // TODO: Add attachments (requires Duralumin)
}

public enum TicketRelationship
{
    Unknown,
    DependantOn,
    ParentOf,
    ClonedFrom,
}