using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Electrum.Domain.Querying;
using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;

namespace Vulpes.Zinc.Core.RegistrationExtensions;

public static class MediationRegistration
{
    public static IServiceCollection InjectMediator(this IServiceCollection services) => services
        .InjectCommands()
        .InjectQueries()
        .InjectMediatorInternal()
        ;

    private static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<LogInCommand>, LogInCommandHandler>()
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        .AddTransient<CommandHandler<CreateNewProjectCommand>, CreateNewProjectCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>, GetUserByLoginCredentialsQueryHandler>()
        .AddTransient<QueryHandler<GetProjectByKeyQuery, Project>, GetProjectByKeyQueryHandler>()
        .AddTransient<QueryHandler<GetAllAccessibleProjectsQuery, IQueryable<Project>>, GetAllAccessibleProjectsQueryHandler>()
        .AddTransient<QueryHandler<GetProjectByShorthandQuery, Project>, GetProjectByShorthandQueryHandler>()
        ;

    private static IServiceCollection InjectMediatorInternal(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<LogInCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<CreateNewProjectCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetProjectByKeyQuery, Project>>())
                .Register(provider.GetRequiredService<QueryHandler<GetAllAccessibleProjectsQuery, IQueryable<Project>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetProjectByShorthandQuery, Project>>())
                ;

            return mediator;
        });

        return services;
    }
}
