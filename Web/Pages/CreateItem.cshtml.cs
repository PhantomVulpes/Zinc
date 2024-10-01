using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class CreateItemModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public CreateItemModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    private readonly static string pageTitle = "Create Ticket";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project.Shorthand);

    public Project Project { get; private set; } = Project.Empty;

    [BindProperty]
    public string ItemName { get; set; } = string.Empty;

    [BindProperty]
    public string ItemDescription { get; set; } = string.Empty;

    public async Task OnGetAsync(string projectShorthand)
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(projectShorthand));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var workItem = WorkItem.Default(Project) with
        {
            Title = ItemName,
            Description = ItemDescription,
            ReporterKey = GetZincUserKey(),
        };

        return RedirectToPage("Project", Project.Shorthand);
    }

    public static Dictionary<string, string> GetBreadcrumbs(string projectShorthand) => ProjectModel.GetBreadcrumbs(projectShorthand).AddAndReturn(pageTitle, "/create-work-item");
}
