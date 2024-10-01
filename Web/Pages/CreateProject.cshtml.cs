using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class CreateProjectModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public CreateProjectModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public static readonly string pageTitle = "Create Project";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    [BindProperty]
    public string ProjectName { get; set; } = string.Empty;

    [BindProperty]
    public string ProjectShorthand { get; set; } = string.Empty;

    [BindProperty]
    public string ProjectDescription { get; set; } = string.Empty;

    [BindProperty]
    public string AllowedUserEmails { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        var emails = string.IsNullOrEmpty(AllowedUserEmails) ? [] : AllowedUserEmails
            .Split(',')
            .Select(value => value.Trim())
            .Where(value => !string.IsNullOrEmpty(value))
            ;

        // TODO: Handle errors. The form should be reloaded with something to tell the user what went wrong.
        // TODO: Add a message to the user.
        await mediator.ExecuteCommandAsync(new CreateNewProjectCommand(ProjectName, ProjectShorthand, ProjectDescription, emails, GetZincUserKey()));

        return RedirectToPage("/index");    // TODO: These paths need to be kept somewhere.
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, "/create-project");
}
