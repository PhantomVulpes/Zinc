using Microsoft.AspNetCore.Http.Extensions;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Web.Middleware;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages.StatusPages;

public class _500Model : ZincPageModel
{
    public string Exception { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    private static readonly string pageTitle = "500 Error";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs(HttpContext);

    public void OnGet()
    {
        Exception = HttpContext.Items[KnownExceptionHandling.ExceptionKey]!.ToString()!;
        Message = HttpContext.Items[KnownExceptionHandling.ExceptionMessageKey]!.ToString()!;
    }

    public static Dictionary<string, string> GetBreadcrumbs(HttpContext context) => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, context.Request.GetEncodedUrl());
}
