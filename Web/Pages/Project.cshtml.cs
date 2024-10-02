using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class ProjectModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public ProjectModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public Project Project { get; private set; } = Project.Empty;
    public IEnumerable<Ticket> Tickets { get; private set; } = [];

    public static readonly string pageTitle = "Project";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project.Shorthand);

    public async Task OnGetAsync(string projectShorthand)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
        Tickets = await mediator.RequestResponseAsync<GetTicketsUnderProject, IEnumerable<Ticket>>(new GetTicketsUnderProject(Project.Key));
    }

    public static Dictionary<string, string> GetBreadcrumbs(string projectShorthand) => ProjectsModel.GetBreadcrumbs().AddAndReturn(pageTitle, $"/projects/{projectShorthand}");
}
