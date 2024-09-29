using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Core.Domain.Mediation;

namespace Vulpes.Zinc.Domain.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection InjectDomain(this IServiceCollection services) => services
        .AutoInjectServices()
        .InjectCommands()
        .InjectQueries()
        .InjectMediator()
        ;

    private static IServiceCollection AutoInjectServices(this IServiceCollection services) =>
        // TODO: Implement this.
        services;


    private static IServiceCollection InjectCommands(this IServiceCollection services) => services;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services;

    private static IServiceCollection InjectMediator(this IServiceCollection services) => services
        .AddTransient<IMediator, Mediator>()
        ;
}
