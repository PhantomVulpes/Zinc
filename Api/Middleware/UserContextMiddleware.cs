using System.Security.Claims;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Api.Middleware;

public class UserContextMiddleware : IMiddleware
{
    public static string ContextKey => $"{nameof(ZincUser)}.{nameof(ZincUser.Key)}".ToLower();

    private readonly IDataRepository<ZincUser> userRepository;

    public UserContextMiddleware(IDataRepository<ZincUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Items[ContextKey] = ZincUser.Empty;
            await next(context);
        }
        else
        {
            var key = Guid.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("UserId")?.Value!);
            var user = await userRepository.GetAsync(key);

            context.Items[ContextKey] = user;
            await next(context);
        }
    }
}
