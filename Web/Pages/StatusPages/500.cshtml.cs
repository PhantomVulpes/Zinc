using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulpes.Zinc.Web.Middleware;

namespace Vulpes.Zinc.Web.Pages.StatusPages;

public class _500Model : PageModel
{
    public string Exception { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    public void OnGet()
    {
        Exception = HttpContext.Items[KnownExceptionHandling.ExceptionKey]!.ToString()!;
        Message = HttpContext.Items[KnownExceptionHandling.ExceptionMessageKey]!.ToString()!;
    }
}
