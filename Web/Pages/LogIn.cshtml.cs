using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Zinc.Domain.Logging;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Web.Middleware;
using Vulpes.Zinc.Web.Models;

namespace Vulpes.Zinc.Web.Pages;

public class LogInModel : ZincPageModel
{
    private readonly IMediator mediator;
    private readonly ILogger<LogInModel> logger;

    public LogInModel(IMediator mediator, ILogger<LogInModel> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [BindProperty]
    public string EnteredUsername { get; set; } = string.Empty;

    [BindProperty]
    public string EnteredPassword { get; set; } = string.Empty;

    public static readonly string pageTitle = "Log In";
    public override string PageTitle => pageTitle;

    public override Dictionary<string, string> Breadcrumbs => GetBreadcrumbs();

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var user = await mediator.RequestResponseAsync<GetUserByLoginCredentials, ZincUser>(new GetUserByLoginCredentials(EnteredUsername, EnteredPassword));

            // Set the user as authenticated
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Key.ToString()),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "UserAuthentication");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal); // Sign in the user

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            HandleException(HttpContext, ex, StatusCodes.Status500InternalServerError);
            throw; // rethrow to trigger exception handling middleware
        }
    }

    private void HandleException(HttpContext context, Exception exception, int status)
    {
        context.Response.StatusCode = status;
        context.Items[KnownExceptionHandling.ExceptionKey] = exception.GetType().Name;
        context.Items[KnownExceptionHandling.ExceptionMessageKey] = exception.Message;
        logger.LogInformation($"{LogTags.Failure} Failed to execute request to {context.Request.Path}. {exception.Message}");
    }

    public static Dictionary<string, string> GetBreadcrumbs() => IndexModel.GetBreadcrumbs().AddAndReturn(pageTitle, "/log-in");
}
