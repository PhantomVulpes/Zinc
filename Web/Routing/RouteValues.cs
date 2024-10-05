namespace Vulpes.Zinc.Web.Routing;

public record ZincRoute(string PageName, object RouteParameters, string Path)
{
    /// <summary>
    /// Allow a user to use a ZincRoute as if it were a string.
    /// </summary>
    public static implicit operator string(ZincRoute route) => route.Path;

    /// <summary>
    /// Return the ZincRoute as a string.
    /// </summary>
    public override string ToString() => this;

    public static ZincRoute Home() => new("Index", new { }, "/");
    public static ZincRoute Register() => new("Register", new { }, "/register");
    public static ZincRoute LogIn() => new("LogIn", new { }, "/log-in");
    public static ZincRoute LogOut() => new("LogOut", new { }, "/log-out");

    // User related
    public static ZincRoute Projects() => new("Projects", new { }, "/projects");
    public static ZincRoute CreateProject() => new("CreateProject", new { }, "/create-project");
    public static ZincRoute Project(string projectShorthand) => new("Project", new { ProjectShorthand = projectShorthand }, $"/projects/{projectShorthand}");
    public static ZincRoute CreateTicket(string projectShorthand) => new("CreateTicket", new { ProjectShorthand = projectShorthand }, $"/projects/{projectShorthand}/create-ticket");
    public static ZincRoute Ticket(string projectShorthand, Guid ticketKey) => new("Ticket", new { ProjectShorthand = projectShorthand, TicketKey = ticketKey }, $"/projects/{projectShorthand}/{ticketKey}");
}
