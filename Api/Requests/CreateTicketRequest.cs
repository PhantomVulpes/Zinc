using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record CreateTicketRequest(string Title, string Description, IEnumerable<string> Labels)
{
    public CreateTicketCommand ToCommand(Guid projectKey, Guid creatorKey) =>
        new(projectKey, Title, Description, Labels, creatorKey);
}