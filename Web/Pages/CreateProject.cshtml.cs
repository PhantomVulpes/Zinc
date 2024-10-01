using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class CreateProjectModel : SecuredZincPageModel
{
    public static readonly string pageTitle = "Create Project";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    [BindProperty]
    public string ProjectName { get; set; } = string.Empty;

    [BindProperty]
    public string ProjectDescription { get; set; } = string.Empty;

    [BindProperty]
    public string AllowedUserEmails { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        var emails = AllowedUserEmails
            .Split(',')
            .Select(value => value.Trim())
            .Where(value => !string.IsNullOrEmpty(value))
            ;

        return RedirectToPage("/index");    // TODO: These paths need to be kept somewhere.
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, "/create-project");
}
