using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vulpes.Zinc.Web.Models;

public abstract class ZincPageModel : PageModel
{
    public abstract string PageTitle { get; }

    public abstract Dictionary<string, string> Breadcrumbs { get; }
}
