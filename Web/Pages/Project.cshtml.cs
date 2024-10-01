using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class ProjectModel : SecuredZincPageModel
{
    private readonly IDataRepository<Project> projectRepository;
    private readonly IMediator mediator;

    public ProjectModel(IDataRepository<Project> projectRepository, IMediator mediator)
    {
        this.projectRepository = projectRepository;
        this.mediator = mediator;
    }

    public Project Project { get; private set; } = Project.Empty;

    public static readonly string pageTitle = "Project";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project.Shorthand);

    public async Task OnGetAsync(string projectShorthand)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
    }

    public static Dictionary<string, string> GetBreadcrumbs(string projectShorthand) => ProjectsModel.GetBreadcrumbs().AddAndReturn(pageTitle, $"/projects/{projectShorthand}");
}
