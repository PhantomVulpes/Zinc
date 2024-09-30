using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;

namespace Vulpes.Zinc.Web.Pages;

public class LogInModel : PageModel
{
    private readonly IMediator mediator;

    public LogInModel(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [BindProperty]
    public string EnteredUsername { get; set; } = string.Empty;

    [BindProperty]
    public string EnteredPassword { get; set; } = string.Empty;


    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var user = await mediator.RequestResponseAsync<GetUserByLoginCredentials, ZincUser>(new GetUserByLoginCredentials(EnteredUsername, EnteredPassword));

            // Authorize user.
            // Set the user as authenticated
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                // Add any additional claims as needed
            };

            var identity = new ClaimsIdentity(claims, "UserAuthentication");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal); // Sign in the user


            return RedirectToPage("Index");
        }
        catch
        {
            return RedirectToPage("RegisterUser");
        }

    }
}
