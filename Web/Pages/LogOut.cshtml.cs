using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

// TODO: This probably doesn't need to be a page but I'm testing other things rn too. Make it just a post from any page.
public class LogOutModel : SecuredZincPageModel
{
    public static readonly string pageTitle = "Log Out";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/index");
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, "/log-out");
}
