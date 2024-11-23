using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Extensions;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;
public class ProjectModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    [BindProperty]
    public string ProjectShorthand { get; set; } = string.Empty;
    public Project Project { get; private set; } = Project.Empty;
    public Dictionary<TicketStatus, IEnumerable<Ticket>> TicketsByStatus { get; private set; } = [];

    public override string PageTitle => Project.Name;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project);

    [BindProperty]
    public string UpdatedLabels { get; set; } = string.Empty;

    public ProjectModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task OnGetAsync(string projectShorthand)
    {
        ProjectShorthand = projectShorthand;
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(ProjectShorthand));
        TicketsByStatus = (await mediator.RequestResponseAsync<GetTicketsUnderProject, IEnumerable<Ticket>>(new GetTicketsUnderProject(Project.Key)))
            .GroupBy(ticket => ticket.Status)
            .OrderBy(group => group.Key, new TicketStatusComparer())
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(ticket => string.Join(", ", ticket.Labels)).AsEnumerable()
            );

        UpdatedLabels = string.Join(", ", Project.Labels);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(ProjectShorthand));

        var labels = UpdatedLabels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(label => label.Trim()).ToList();
        await mediator.ExecuteCommandAsync(new UpdateProjectLabelsCommand(Project.Key, labels, GetZincUserKey()));

        return this.RedirectWithZincRoutes(ZincRoute.Project(Project.Shorthand));
    }

    public static Dictionary<string, string> GetBreadcrumbs(Project project) => ProjectsModel.GetBreadcrumbs().AddAndReturn(project.Name, ZincRoute.Project(project.Shorthand));

    private class TicketStatusComparer : IComparer<TicketStatus>
    {
        private static readonly List<TicketStatus> StatusHierarchy =
        [
            TicketStatus.InProgress,
            TicketStatus.Open,
            TicketStatus.InReview,
            TicketStatus.Complete,
            TicketStatus.Cancelled,
        ];

        public int Compare(TicketStatus x, TicketStatus y)
        {
            var indexX = StatusHierarchy.IndexOf(x);
            var indexY = StatusHierarchy.IndexOf(y);
            return indexX.CompareTo(indexY);
        }
    }
}