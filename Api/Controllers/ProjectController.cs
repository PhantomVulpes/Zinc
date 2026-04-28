using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Api.Requests;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;

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

    [HttpGet("projects")]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Project>>> GetAllAccessibleProjectsAsync()
    {
        var projects = await mediator.RequestResponseAsync(new GetAllAccessibleProjectsQuery(RegisteredUser.Key));
        return Ok(projects);
    }

    [HttpGet("projects/{projectShorthand}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    public async Task<ActionResult<Project>> GetProjectByShorthandAsync(string projectShorthand)
    {
        var project = await mediator.RequestResponseAsync(new GetProjectByShorthandQuery(projectShorthand, RegisteredUser.Key));
        return Ok(project);
    }

    [HttpPost("ticket/create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Project>> CreateNewTicketAsync(CreateTicketRequest request)
    {
        await mediator.ExecuteCommandAsync(request.ToCommand(RegisteredUser.Key));

        return Ok();
    }

    [HttpPost("ticket/add-comment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> AddCommentToTicketAsync(AddCommentToTicketRequest request)
    {
        await mediator.ExecuteCommandAsync(request.ToCommand(RegisteredUser.Key));

        return Ok();
    }

    [HttpPost("ticket/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> EditTicketAsync(EditTicketRequest request)
    {
        await mediator.ExecuteCommandAsync(request.ToCommand(RegisteredUser.Key));

        return Ok();
    }

    [HttpPost("projects/edit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> EditProjectAsync(EditProjectRequest request)
    {
        await mediator.ExecuteCommandAsync(request.ToCommand(RegisteredUser.Key));

        return Ok();
    }
}
