using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vulpes.Zinc.Web.Pages;
public class IndexModel : PageModel
{
    public string LoggedUsername { get; private set; } = string.Empty;
    private readonly ILogger<IndexModel> logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        this.logger = logger;
    }

    public async Task OnGetAsync() => LoggedUsername = HttpContext.User.Identity.Name!;
}
