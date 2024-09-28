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
    public User AssignedTo { get; init; } = User.Empty;
    public User Reporter { get; init; } = User.Empty;

    public WorkItemStatus Status { get; init; } = WorkItemStatus.Unknown;
}