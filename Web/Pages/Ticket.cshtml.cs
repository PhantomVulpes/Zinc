using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;

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

    public Project Project { get; private set; } = Project.Empty;
    public Ticket Ticket { get; private set; } = Ticket.Empty;

    public async Task OnGetAsync(string projectShorthand, Guid ticketKey)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
        Ticket = await mediator.RequestResponseAsync<GetTicketByKey, Ticket>(new GetTicketByKey(ticketKey));
    }

    public static Dictionary<string, string> GetBreadcrumbs(string projectShorthand, Guid ticketKey) => ProjectModel.GetBreadcrumbs(projectShorthand).AddAndReturn(pageTitle, $"/projects/{projectShorthand}/{ticketKey}");
}
