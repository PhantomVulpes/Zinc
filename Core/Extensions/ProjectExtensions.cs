using Vulpes.Electrum.Domain.Validation;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Extensions;

public static class ProjectExtensions
{
    public static Project WithAddedTicket(this Project project, IValidationModel<Ticket> validationTicket)
    {
        validationTicket.ThrowIfInvalid();
        var ticket = validationTicket.Value;
        var tickets = project.Tickets.Union([ticket]).OrderBy(ticket => ticket.Index);

        return project with
        {
            Tickets = tickets
        };
    }

    public static Project WithUpdatedTicket(this Project project, IValidationModel<Ticket> updatedValidationTicket)
    {
        updatedValidationTicket.ThrowIfInvalid();
        var updatedTicket = updatedValidationTicket.Value;

        var tickets = project.Tickets.Select(ticket => ticket.Index == updatedTicket.Index ? updatedTicket : ticket);

        return project with
        {
            Tickets = tickets
        };
    }
}