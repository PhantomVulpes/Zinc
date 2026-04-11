using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Mediation;
using Vulpes.Zinc.Core.Commands;

namespace Vulpes.Zinc.Core.RegistrationExtensions;

public static class MediationRegistration
{
    public static IServiceCollection InjectMediator(this IServiceCollection services) => services
        .InjectCommands()
        .InjectMediatorInternal()
        ;

    private static IServiceCollection InjectCommands(this IServiceCollection services) => services
        .AddTransient<CommandHandler<RegisterNewUserCommand>, RegisterNewUserCommandHandler>()
        ;

    private static IServiceCollection InjectMediatorInternal(this IServiceCollection services)
    {
        _ = services.AddTransient<IMediator>((provider) =>
        {
            var mediator = new Mediator();

            _ = mediator
                .Register(provider.GetRequiredService<CommandHandler<RegisterNewUserCommand>>())
                ;

            _ = mediator
                // .Register(provider.GetRequiredService<QueryHandler<GetUserByLoginCredentialsQuery, RegisteredUser>>())
                ;

            return mediator;
        });

        return services;
    }
}
