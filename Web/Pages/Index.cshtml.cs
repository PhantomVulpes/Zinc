using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;
public class IndexModel : ZincPageModel
{
    public string LoggedUsername { get; private set; } = string.Empty;

    private static readonly string pageTitle = "Home";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    // TODO: Add logged user to layout with view components.
    public async Task OnGetAsync() => LoggedUsername = HttpContext.User.Identity!.Name!;

    public static Dictionary<string, string> GetBreadcrumbs() => new() { { pageTitle, "/index" } };
}
