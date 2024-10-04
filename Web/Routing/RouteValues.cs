namespace Vulpes.Zinc.Web.Routing;

public static class RouteValues
{
    public static object Project(string projectShorthand) => new { ProjectShorthand = projectShorthand };
    public static object Ticket(string projectShorthand, Guid ticketKey) => new { ProjectShorthand = projectShorthand, TicketKey = ticketKey };
}
