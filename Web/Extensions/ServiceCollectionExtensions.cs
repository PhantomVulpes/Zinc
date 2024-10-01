using Vulpes.Zinc.Web.Middleware;

namespace Vulpes.Zinc.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiddlewareServices(this IServiceCollection services) => services
        .AddTransient<KnownExceptionHandling>()
        ;

    public static IApplicationBuilder AddMiddleware(this IApplicationBuilder applicationBuilder) => applicationBuilder
        .UseMiddleware<KnownExceptionHandling>()
        ;
}
