using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Web.Pages;
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> logger;
    private readonly IDataRepository<Project> dataRepository;

    public IndexModel(ILogger<IndexModel> logger, IDataRepository<Project> dataRepository)
    {
        this.logger = logger;
        this.dataRepository = dataRepository;
    }

    public async Task OnGetAsync()
    {
        var item = Project.Default with
        {
            Name = "Testing"
        };

        await dataRepository.InsertAsync(item);
    }
}
