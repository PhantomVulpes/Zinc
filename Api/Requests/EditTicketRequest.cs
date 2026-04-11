using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Api.Requests;

public record EditTicketRequest(
    Guid ProjectKey,
    int TicketIndex,
    string Title,
    string Description,
    IEnumerable<string> Labels,
    TicketStatus Status)
{
    public EditTicketCommand ToCommand(Guid userKey) =>
        new(ProjectKey, TicketIndex, Title, Description, Labels, Status, userKey);
}
