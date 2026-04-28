using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record CreateTicketRequest(Guid ProjectKey, string Title, string Description, IEnumerable<string> Labels)
{
    public CreateTicketCommand ToCommand(Guid creatorKey) =>
        new(ProjectKey, Title, Description, Labels, creatorKey);
}