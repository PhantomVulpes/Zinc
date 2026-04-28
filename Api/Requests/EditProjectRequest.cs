using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Api.Requests;

public record EditProjectRequest(Guid ProjectKey, string ProjectName, string Description, TicketStatus DefaultTicketStatus, IEnumerable<Guid> AllowedUserKeys, ProjectStatus ProjectStatus, IEnumerable<string> Labels)
{
    public EditProjectCommand ToCommand(Guid userKey) =>
        new(ProjectKey, userKey, ProjectName, Description, DefaultTicketStatus, AllowedUserKeys, ProjectStatus, Labels);
}
