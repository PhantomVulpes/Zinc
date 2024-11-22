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

public class TicketModel : SecuredZincPageModel
{
    private readonly IMediator mediator;
    private readonly IDataRepository<ZincUser> userRepository;

    public TicketModel(IMediator mediator, IDataRepository<ZincUser> userRepository)
    {
        this.mediator = mediator;
        this.userRepository = userRepository;
    }

    public override string PageTitle => Ticket.Title;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(Project, Ticket);

    [BindProperty]
    public string ProjectShorthand { get; set; } = string.Empty;
    public Project Project { get; private set; } = Project.Empty;

    [BindProperty]
    public Guid TicketKey { get; set; } = Guid.Empty;
    public Ticket Ticket { get; private set; } = Ticket.Empty;

    [BindProperty]
    public string NewStatus { get; set; } = string.Empty;

    [BindProperty]
    public string UpdatedDescription { get; set; } = string.Empty;

    [BindProperty]
    public string Comment { get; set; } = string.Empty;

    public string LabelsJoined { get; set; } = string.Empty;

    [BindProperty]
    public string UpdatedLabels { get; set; } = string.Empty;

    public async Task OnGetAsync(string projectShorthand, Guid ticketKey)
    {
        ProjectShorthand = projectShorthand;
        TicketKey = ticketKey;

        await LoadProperties();

        UpdatedDescription = Ticket.Description;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadProperties();

        if (HttpContext.Request.Form.ContainsKey(PostAction.AddComment.ToString()))
        {
            await mediator.ExecuteCommandAsync(new AddTicketCommentCommand(Comment, TicketKey, GetZincUserKey()));
        }
        else if (HttpContext.Request.Form.ContainsKey(PostAction.UpdateLabels.ToString()))
        {
            var labels = UpdatedLabels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(label => label.Trim()).ToList();
            // await mediator.ExecuteCommandAsync(new UpdateTicketLabelsCommand(TicketKey, labels, GetZincUserKey()));
        }
        else
        {
            await UpdateStatus();
            await mediator.ExecuteCommandAsync(new UpdateTicketDescriptionCommand(TicketKey, UpdatedDescription, GetZincUserKey()));
        }

        return this.RedirectWithZincRoutes(ZincRoute.Ticket(Project.Shorthand, Ticket.Key));
    }

    public async Task LoadProperties()
    {
        Project = await mediator.RequestResponseAsync<GetProjectByShorthand, Project>(new GetProjectByShorthand(ProjectShorthand));
        Ticket = await mediator.RequestResponseAsync<GetTicketByKey, Ticket>(new GetTicketByKey(TicketKey));
        LabelsJoined = string.Join(", ", Ticket.Labels);
    }

    public async Task<string> GetCommentAuthorName(Guid authorKey) => (await userRepository.GetAsync(authorKey)).Username;

    private async Task UpdateStatus()
    {
        var isSuccessful = Enum.TryParse<TicketStatus>(NewStatus, out var newStatus);
        if (!isSuccessful)
        {
            newStatus = Ticket.Status;
        }

        await mediator.ExecuteCommandAsync(new UpdateTicketStatusCommand(TicketKey, newStatus, GetZincUserKey()));
    }

    public static Dictionary<string, string> GetBreadcrumbs(Project project, Ticket ticket) => ProjectModel.GetBreadcrumbs(project).AddAndReturn(ticket.Title, ZincRoute.Ticket(project.Shorthand, ticket.Key));

    public enum PostAction
    {
        UpdateTicket,
        AddComment,
        UpdateLabels
    }
}
