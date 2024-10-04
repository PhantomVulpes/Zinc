namespace Vulpes.Zinc.Web.Routing;

public record ZincRoute(string PageName, object RouteParameters)
{
    public static ZincRoute Project(string projectShorthand) => new("Project", new { ProjectShorthand = projectShorthand });
    public static ZincRoute Ticket(string projectShorthand, Guid ticketKey) => new("Ticket", new { ProjectShorthand = projectShorthand, TicketKey = ticketKey });
}
