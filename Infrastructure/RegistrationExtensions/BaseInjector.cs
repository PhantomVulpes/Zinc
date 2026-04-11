using Microsoft.Extensions.DependencyInjection;

namespace Vulpes.Zinc.Infrastructure.RegistrationExtensions;

public static class BaseInjector
{
    public static IServiceCollection InjectInfrastructure(this IServiceCollection services) => services
        .InjectMongoServices()
        ;
}
