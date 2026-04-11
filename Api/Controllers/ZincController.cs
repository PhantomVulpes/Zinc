using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Zinc.Api.Middleware;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class ZincController : ControllerBase
{
    public RegisteredUser RegisteredUser => (HttpContext.Items[UserContextMiddleware.ContextKey] as RegisteredUser)!;
}