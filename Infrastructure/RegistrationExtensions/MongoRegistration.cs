using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Infrastructure.Mongo;

namespace Vulpes.Zinc.Infrastructure.RegistrationExtensions;

public static class MongoRegistration
{
    public static IServiceCollection InjectMongoServices(this IServiceCollection services) => services
        .InjectMongoServicesInernal()
        .InjectQueryProviders()
        ;

    private static IServiceCollection InjectMongoServicesInernal(this IServiceCollection services) => services
        .AddSingleton<IMongoProvider, MongoProvider>()
        .AddTransient(typeof(IModelRepository<>), typeof(MongoRepository<>))
        ;

    private static IServiceCollection InjectQueryProviders(this IServiceCollection services) => services
        .AddTransient<IQueryProvider<RegisteredUser>, MongoQueryProvider<RegisteredUser>>()
        ;
}