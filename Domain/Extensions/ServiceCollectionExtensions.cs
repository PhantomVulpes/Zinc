﻿using Microsoft.Extensions.DependencyInjection;
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
        .AddTransient<CommandHandler<CreateTicketCommand>, CreateTicketCommandHandler>()
        .AddTransient<CommandHandler<UpdateTicketStatusCommand>, UpdateTicketStatusCommandHandler>()
        .AddTransient<CommandHandler<UpdateTicketDescriptionCommand>, UpdateTicketDescriptionCommandHandler>()
        .AddTransient<CommandHandler<AddTicketCommentCommand>, AddTicketCommentCommandHandler>()
        .AddTransient<CommandHandler<UpdateTicketLabelsCommand>, UpdateTicketLabelsCommandHandler>()
        .AddTransient<CommandHandler<UpdateProjectLabelsCommand>, UpdateProjectLabelsCommandHandler>()
        .AddTransient<CommandHandler<DeleteTicketFromDatabaseCommand>, DeleteTicketFromDatabaseCommandHandler>()
        ;

    private static IServiceCollection InjectQueries(this IServiceCollection services) => services
        .AddTransient<QueryHandler<GetUserByLoginCredentials, ZincUser>, GetUserByLoginCredentialsHandler>()
        .AddTransient<QueryHandler<GetProjectsForUser, IEnumerable<Project>>, GetProjectsForUserHandler>()
        .AddTransient<QueryHandler<GetProjectByShorthand, Project>, GetProjectByShorthandHandler>()
        .AddTransient<QueryHandler<GetTicketsUnderProject, IEnumerable<Ticket>>, GetTicketsUnderProjectHandler>()
        .AddTransient<QueryHandler<GetTicketByKey, Ticket>, GetTicketByKeyHandler>()
        .AddTransient<QueryHandler<GetTicketStatsByProject, Dictionary<TicketStatus, int>>, GetTicketStatsByProjectHandler>()
        .AddTransient<QueryHandler<GetUserByKey, ZincUser>, GetUserByKeyHandler>()
        ;

    private static IServiceCollection InjectMediator(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<CreateNewProjectCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<CreateTicketCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<UpdateTicketStatusCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<UpdateTicketDescriptionCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<AddTicketCommentCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<UpdateTicketLabelsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<UpdateProjectLabelsCommand>>())
                .Register(provider.GetRequiredService<CommandHandler<DeleteTicketFromDatabaseCommand>>())
                ;

            _ = mediator
                .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentials, ZincUser>>())
                .Register(provider.GetRequiredService<QueryHandler<GetProjectsForUser, IEnumerable<Project>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetProjectByShorthand, Project>>())
                .Register(provider.GetRequiredService<QueryHandler<GetTicketsUnderProject, IEnumerable<Ticket>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetTicketByKey, Ticket>>())
                .Register(provider.GetRequiredService<QueryHandler<GetTicketStatsByProject, Dictionary<TicketStatus, int>>>())
                .Register(provider.GetRequiredService<QueryHandler<GetUserByKey, ZincUser>>())
                ;

            return mediator;
        });

        return services;
    }
}
