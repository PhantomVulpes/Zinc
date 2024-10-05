using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Pages;
public class IndexModel : ZincPageModel
{
    public string LoggedUsername { get; private set; } = string.Empty;

    private static readonly string pageTitle = "Home";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    // TODO: Add logged user to layout with view components.
    public void OnGet()
    {
        var defaultUsers = new List<ZincUser>();
        for (var i = 0; i < 10; i++)
        {
            defaultUsers.Add(ZincUser.Default);
        }

        LoggedUsername = HttpContext.User.Identity!.Name!;
    }

    public static Dictionary<string, string> GetBreadcrumbs() => new() { { pageTitle, ZincRoute.Home() } };
}
