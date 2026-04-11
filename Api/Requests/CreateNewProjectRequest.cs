using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record CreateNewProjectRequest(string ProjectName, string ProjectShorthand, string ProjectDescription)
{
    public CreateNewProjectCommand ToCommand(Guid projectKey, Guid userKey) => new(projectKey, ProjectName, ProjectShorthand, ProjectDescription, userKey);
}
