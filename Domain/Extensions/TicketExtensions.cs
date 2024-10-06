using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Extensions;
public static class TicketExtensions
{
    public static string ToLogName(this Ticket ticket) => $"Ticket {ticket.Key} ({ticket.Title})";
}
