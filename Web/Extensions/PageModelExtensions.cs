using Microsoft.AspNetCore.Mvc;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

namespace Vulpes.Zinc.Web.Extensions;

public static class PageModelExtensions
{
    public static RedirectToPageResult RedirectWithZincRoutes(this ZincPageModel pageModel, ZincRoute route) => pageModel.RedirectToPage(route.PageName, route.RouteParameters);
}
