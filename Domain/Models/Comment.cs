namespace Vulpes.Zinc.Domain.Models;
public record Comment
{
    public static Comment Empty => new();
    public static Comment Default =>
        Empty with
        {
            CreatedDate = DateTime.UtcNow,
        };

    public string Value { get; init; } = string.Empty;
    public Guid Author { get; init; } = Guid.Empty;
    public DateTime CreatedDate { get; init; } = DateTime.MinValue;
}
