using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Requests;

public record AddCommentToTicketRequest(Guid ProjectKey, int TicketIndex, string Comment)
{
    public AddCommentToTicketCommand ToCommand(Guid userKey) => new(ProjectKey, TicketIndex, Comment, userKey);
}