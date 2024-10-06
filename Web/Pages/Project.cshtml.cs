using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;

public class ProjectModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public ProjectModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Project Project { get; private set; } = Project.Empty;
    public Dictionary<TicketStatus, IEnumerable<Ticket>> TicketsByStatus { get; private set; } = [];

    public override string PageTitle => Project.Name;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project);

    public async Task OnGetAsync(string projectShorthand)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
        TicketsByStatus = (await mediator.RequestResponseAsync<GetTicketsUnderProject, IEnumerable<Ticket>>(new GetTicketsUnderProject(Project.Key)))
            .GroupBy(ticket => ticket.Status)
            .ToDictionary(group => group.Key, group => group.AsEnumerable());
    }

    public static Dictionary<string, string> GetBreadcrumbs(Project project) => ProjectsModel.GetBreadcrumbs().AddAndReturn(project.Name, ZincRoute.Project(project.Shorthand));
}
