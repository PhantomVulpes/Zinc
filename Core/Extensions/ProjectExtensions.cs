using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Extensions;

public static class ProjectExtensions
{
    public static Project WithAddedTicket(this Project project, Ticket ticket)
    {
        var tickets = project.Tickets.Union([ticket]).OrderBy(ticket => ticket.Index);

        return project with
        {
            Tickets = tickets
        };
    }
}