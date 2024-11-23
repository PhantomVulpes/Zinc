using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;

public class ProjectsModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public ProjectsModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public static readonly string pageTitle = "Projects";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    public IEnumerable<Project> AccessibleProjects { get; private set; } = [];

    public async Task OnGetAsync()
    {
        AccessibleProjects = await mediator.RequestResponseAsync<GetProjectsForUser, IEnumerable<Project>>(new GetProjectsForUser(GetZincUserKey()));
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, ZincRoute.Projects());

    public async Task<Dictionary<TicketStatus, int>> GetTicketStats(Guid projectKey) =>
        await mediator.RequestResponseAsync<GetTicketStatsByProject, Dictionary<TicketStatus, int>>(new(projectKey, GetZincUserKey()));

    public async Task<ZincUser> GetProjectCreator(Guid projectKey) =>
        await mediator.RequestResponseAsync<GetUserByKey, ZincUser>(new(AccessibleProjects.First(project => project.Key == projectKey).CreatorKey));
}
