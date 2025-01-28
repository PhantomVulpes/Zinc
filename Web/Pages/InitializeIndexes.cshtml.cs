using Amazon.Runtime.Internal.Auth;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Domain.Security;
using Vulpes.Zinc.Web.Extensions;
using Vulpes.Zinc.Web.Models;
using Vulpes.Zinc.Web.Routing;

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
        await LoadPropertiesAsync();

        if (User.Role != Role.Admin)
        {
            throw new UnauthorizedAccessException("You are not authorized to access this page.");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadPropertiesAsync();
        await mediator.ExecuteCommandAsync(new InitializeAllIndexesCommand(User));
        
        return this.RedirectWithZincRoutes(ZincRoute.Home());
    }

    private async Task LoadPropertiesAsync()
    {
        User = await mediator.RequestResponseAsync<GetUserByKey, ZincUser>(new(GetZincUserKey()));
    }
    public override Dictionary<string, string> Breadcrumbs => IndexModel.GetBreadcrumbs().AddAndReturn(PageTitle, "/InitializeIndexes");
}