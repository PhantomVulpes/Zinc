using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Api.Requests;
using Vulpes.Zinc.Api.Responses;
using Vulpes.Zinc.Api.Services;
using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Api.Controllers;

/// <summary>
/// User controller for API v0.8 (in development)
/// This is where you add new features and breaking changes
/// </summary>
[ApiVersion("0.8")]
public class UserControllerV08 : ZincController
{
    private readonly IMediator mediator;
    private readonly IJwtTokenService jwtTokenService;

    public UserControllerV08(IMediator mediator, IJwtTokenService jwtTokenService)
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

    // Example: Add new v0.8-only endpoint
    // [HttpGet("profile")]
    // [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
    // public async Task<ActionResult<UserProfile>> GetUserProfileAsync()
    // {
    //     // New feature only in v0.8
    //     return Ok(new UserProfile());
    // }
}
