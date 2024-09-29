using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Security;

namespace Vulpes.Zinc.Web.Pages;

public class RegisterUserModel : PageModel
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

        // Verify the email isn't in the database.
        // Post the user.
        await repository.InsertAsync(user);

        return RedirectToPage("Index");
    }
}
