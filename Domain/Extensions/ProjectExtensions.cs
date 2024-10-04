using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Extensions;
public static class ProjectExtensions
{
    public static string ToLogString(this Project project)
    {
        return $"Project {project.Key} ({project.Shorthand})";
    }
}
