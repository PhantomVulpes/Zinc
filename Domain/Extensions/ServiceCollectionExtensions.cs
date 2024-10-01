using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Mediation;
using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;

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


    private static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<CreateNewProjectCommand>, CreateNewProjectCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentials, ZincUser>, GetUserByLoginCredentialsHandler>()
        .AddTransient<QueryHandler<GetProjectsForUser, IEnumerable<Project>>, GetProjectsForUserHandler>()
        ;

    private static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<CreateNewProjectCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentials, ZincUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetProjectsForUser, IEnumerable<Project>>>())
                ;

            return mediator;
        });

        return services;
    }
}
