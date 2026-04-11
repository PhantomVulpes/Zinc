using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Api.Requests;
using Vulpes.Zinc.Api.Responses;
using Vulpes.Zinc.Api.Services;
using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Controllers;

public class UserController : ZincController
{
    private readonly IMediator mediator;
    private readonly IJwtTokenService jwtTokenService;

    public UserController(IMediator mediator, IJwtTokenService jwtTokenService)
    {
        this.mediator = mediator;
        this.jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> RegisterUserAsync(RegisterNewUserRequest request)
    {
        var command = request.ToCommand();
        await mediator.ExecuteCommandAsync(command);

        return Ok(command.Key.ToString());
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var query = request.ToQuery();
        var user = await mediator.RequestResponseAsync(query);

        var token = jwtTokenService.GenerateToken(user);

        var response = LoginResponse.FromRegisteredUser(user, token);

        await mediator.ExecuteCommandAsync(new LogInCommand(user));

        return Ok(response);
    }
}
