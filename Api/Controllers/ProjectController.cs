using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Api.Requests;

namespace Vulpes.Zinc.Api.Controllers;

public class ProjectController : ZincController
{
    private readonly IMediator mediator;

    public ProjectController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("projects/create")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> CreateProjectAsync(CreateNewProjectRequest request)
    {
        var projectKey = Guid.NewGuid();
        await mediator.ExecuteCommandAsync(request.ToCommand(projectKey, RegisteredUser.Key));

        return Ok(projectKey.ToString());
    }
}
