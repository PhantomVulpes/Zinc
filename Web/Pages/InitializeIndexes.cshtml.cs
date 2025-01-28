using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Domain.Security;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;
public class InitializeIndexesModel : SecuredZincPageModel
{
    private readonly IMediator mediator;

    public override string PageTitle => "Initialize Indexes";

    public ZincUser User { get; private set; } = ZincUser.Empty;
    public IEnumerable<IIndexDefinition> Indexes { get; private set; } = [];

    public InitializeIndexesModel(IMediator mediator, IEnumerable<IIndexDefinition> indexes)
    {
        this.mediator = mediator;
        Indexes = indexes;
    }

    public async Task OnGetAsync()
    {
        // Verify the user is an admin.
        User = await mediator.RequestResponseAsync<GetUserByKey, ZincUser>(new(GetZincUserKey()));

        if (User.Role != Role.Admin)
        {
            throw new UnauthorizedAccessException("You are not authorized to access this page.");
        }
    }

    public override Dictionary<string, string> Breadcrumbs => IndexModel.GetBreadcrumbs().AddAndReturn(PageTitle, "/InitializeIndexes");
}