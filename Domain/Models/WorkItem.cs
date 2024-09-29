namespace Vulpes.Zinc.Domain.Models;
public record WorkItem : AggregateRoot
{
    public static WorkItem Empty => new();
    public static WorkItem Default(Project project) =>
        Empty with
        {
            Status = project.DefaultWorkItemStatus
        };

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public ZincUser AssignedTo { get; init; } = ZincUser.Empty;
    public ZincUser Reporter { get; init; } = ZincUser.Empty;

    public WorkItemStatus Status { get; init; } = WorkItemStatus.Unknown;
}