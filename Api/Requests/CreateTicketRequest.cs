using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record CreateTicketRequest(string Title, string Description)
{
    public CreateTicketCommand ToCommand(Guid projectKey, Guid creatorKey)
    {
        return new CreateTicketCommand(projectKey, Title, Description, creatorKey);
    }
}