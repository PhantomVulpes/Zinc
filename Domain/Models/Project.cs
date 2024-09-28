namespace Vulpes.Zinc.Domain.Models;
public record Project : AggregateRoot
{
    public static Project Empty => new();
    public static Project Default { get; } = Empty with
    {
        DefaultWorkItemStatus = WorkItemStatus.InReview
    };

    public string Name { get; init; } = string.Empty;

    public WorkItemStatus DefaultWorkItemStatus { get; init; } = WorkItemStatus.Unknown;
}