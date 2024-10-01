namespace Vulpes.Zinc.Domain.Models;
public record WorkItem : AggregateRoot
{
    public static WorkItem Empty => new();
    public static WorkItem Default(Project project) =>
        Empty with
        {
            Key = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            Status = project.DefaultWorkItemStatus,
        };

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid AssignedToKey { get; init; } = Guid.Empty;
    public Guid ReporterKey { get; init; } = Guid.Empty;
    public IEnumerable<string> Comments { get; init; } = [];
    public DateTime CreatedDate { get; init; } = DateTime.MinValue;
    public DateTime CompletedDate { get; init; } = DateTime.MinValue;

    public Dictionary<WorkItemRelationship, Guid> WorkItemRelationships { get; init; } = [];

    public WorkItemStatus Status { get; init; } = WorkItemStatus.Unknown;

    // TODO: Add labels, attachments (requires Duralumin)
}

public enum WorkItemRelationship
{
    Unknown,
    DependantOn,
    ParentOf,
    ClonedFrom,
}