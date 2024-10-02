namespace Vulpes.Zinc.Domain.Models;
public record Ticket : AggregateRoot
{
    // TODO: Add convention for null serialization from database. Changed name of thing from database, now it breaks unless I make a new one.
    public static Ticket Empty => new();
    public static Ticket Default(Project project) =>
        Empty with
        {
            Key = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            Status = project.DefaultTicketStatus,
        };

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid AssignedToKey { get; init; } = Guid.Empty;
    public Guid ReporterKey { get; init; } = Guid.Empty;
    public IEnumerable<string> Comments { get; init; } = [];
    public DateTime CreatedDate { get; init; } = DateTime.MinValue;
    public DateTime CompletedDate { get; init; } = DateTime.MinValue;

    public Dictionary<TicketRelationship, Guid> WorkItemRelationships { get; init; } = [];

    public TicketStatus Status { get; init; } = TicketStatus.Unknown;

    // TODO: Add labels, attachments (requires Duralumin)
}

public enum TicketRelationship
{
    Unknown,
    DependantOn,
    ParentOf,
    ClonedFrom,
}