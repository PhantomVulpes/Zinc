using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Security;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class RegisterUserModel : ZincPageModel
{
    private readonly IDataRepository<ZincUser> repository;
    private readonly IKnoxHasher knoxHasher;

    public RegisterUserModel(IDataRepository<ZincUser> repository, IKnoxHasher knoxHasher)
    {
        this.repository = repository;
        this.knoxHasher = knoxHasher;
    }

    [BindProperty]
    public string EnteredUsername { get; set; } = string.Empty;

    [BindProperty]
    public string EnteredPassword { get; set; } = string.Empty;

    [BindProperty]
    public string EnteredEmail { get; set; } = string.Empty;

    public static readonly string pageTitle = "Register";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    public async Task<IActionResult> OnPostAsync()
    {
        var passwordHash = knoxHasher.HashPassword(EnteredPassword);
        var user = ZincUser.Default with
        {
            Username = EnteredUsername,
            PasswordHash = passwordHash,
            Role = Role.Basic,
            Email = EnteredEmail
        };

        // TODO: Not even close to done with this.
        // Verify the email isn't in the database with an index.
        // Post the user.
        await repository.InsertAsync(user);

        return RedirectToPage("Index");
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, "/register");
}
