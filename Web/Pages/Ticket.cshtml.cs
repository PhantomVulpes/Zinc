using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;

public class TicketModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public TicketModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    private static readonly string pageTitle = "Ticket";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project.Shorthand, Ticket.Key);

    [BindProperty]
    public string ProjectShorthand { get; set; } = string.Empty;
    public Project Project { get; private set; } = Project.Empty;

    [BindProperty]
    public Guid TicketKey { get; set; } = Guid.Empty;
    public Ticket Ticket { get; private set; } = Ticket.Empty;

    [BindProperty]
    public string NewStatus { get; set; } = string.Empty;

    public async Task OnGetAsync(string projectShorthand, Guid ticketKey)
    {
        ProjectShorthand = projectShorthand;
        TicketKey = ticketKey;

        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(ProjectShorthand));
        Ticket = await mediator.RequestResponseAsync<GetTicketByKey, Ticket>(new GetTicketByKey(TicketKey));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var newStatus = Enum.Parse<TicketStatus>(NewStatus);
        await mediator.ExecuteCommandAsync(new UpdateTicketStatusCommand(TicketKey, newStatus, GetZincUserKey()));

        return RedirectToPage("Ticket", RouteValues.Ticket(ProjectShorthand, TicketKey));
    }

    public static Dictionary<string, string> GetBreadcrumbs(string projectShorthand, Guid ticketKey) => ProjectModel.GetBreadcrumbs(projectShorthand).AddAndReturn(pageTitle, $"/projects/{projectShorthand}/{ticketKey}");
}
