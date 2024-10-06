using Microsoft.AspNetCore.Mvc;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Web.Pages.Components;

public record TicketOverviewModel(Project Project, Ticket Ticket);
public class TicketOverviewViewComponent : ViewComponent
{

    private readonly IDataRepository<Project> projectRepository;

    public TicketOverviewViewComponent(IDataRepository<Project> projectRepository)
    {
        this.projectRepository = projectRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync(Ticket ticket)
    {
        var project = await projectRepository.GetAsync(ticket.ProjectKey);
        var result = new TicketOverviewModel(project, ticket);

        return View("Components/TicketOverviewViewComponent.cshtml", result);
    }
}
