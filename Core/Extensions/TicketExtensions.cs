using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Extensions;

public static class TicketExtensions
{
    public static Ticket WithAddedComment(this Ticket ticket, Comment comment)
    {
        var comments = ticket.Comments.Append(comment);
        return ticket with
        {
            Comments = comments,
        };
    }
}