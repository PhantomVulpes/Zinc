using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Extensions;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;

public class CreateTicketModel : SecuredZincPageModel
{
    private readonly IMediator mediator;
    private readonly IDataRepository<Project> projectRepository;

    public CreateTicketModel(IMediator mediator, IDataRepository<Project> projectRepository)
    {
        this.mediator = mediator;
        this.projectRepository = projectRepository;
    }

    private readonly static string pageTitle = "Create Ticket";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project);

    [BindProperty] // TODO: I know there's a way to bind it to the Project instead, I did it in SEAM with Batches I think. 
    public Guid ProjectKey { get; set; } = Guid.Empty;
    public Project Project { get; private set; } = Project.Empty;

    [BindProperty]
    public string TicketName { get; set; } = string.Empty;

    [BindProperty]
    public string TicketDescription { get; set; } = string.Empty;

    public async Task OnGetAsync(string projectShorthand)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
        ProjectKey = Project.Key;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Project = await projectRepository.GetAsync(ProjectKey);
        await mediator.ExecuteCommandAsync(new CreateTicketCommand(Project.Key, TicketName, TicketDescription, GetZincUserKey()));

        return this.RedirectWithZincRoutes(ZincRoute.Project(Project.Shorthand));
    }

    public static Dictionary<string, string> GetBreadcrumbs(Project project) => ProjectModel.GetBreadcrumbs(project).AddAndReturn(pageTitle, "/create-ticket");
}
