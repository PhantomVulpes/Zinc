using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vulpes.Zinc.Web.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        this.logger = logger;
    }

    public async Task OnGetAsync()
    { }
}
