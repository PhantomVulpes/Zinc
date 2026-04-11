using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Security;

namespace Vulpes.Zinc.Core.RegistrationExtensions;

public static class BaseInjector
{
    public static IServiceCollection InjectCore(this IServiceCollection services) => services
        .AddTransient<IKnoxHasher, KnoxHasher>()
        .InjectMediator()
        ;
}
