using Microsoft.Extensions.DependencyInjection;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.External.Mongo;

namespace Vulpes.Zinc.External.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection InjectExternal(this IServiceCollection services) => services
        .AutoInjectServices()
        .InjectRepositories()
        ;

    private static IServiceCollection AutoInjectServices(this IServiceCollection services) =>
        // TODO: Obviously this is not auto. Fix that.
        services
        .AddTransient<IMongoProvider, MongoProvider>()
        .AddTransient<IKnoxHasher, KnoxHasher>()
        ;

    private static IServiceCollection InjectRepositories(this IServiceCollection services) => services
        .AddTransient<IDataRepository<ZincUser>, MongoRepository<ZincUser>>()
        .AddTransient<IDataRepository<Project>, MongoRepository<Project>>()
        .AddTransient<IDataRepository<WorkItem>, MongoRepository<WorkItem>>()

        .AddTransient<IQueryProvider<ZincUser>, MongoQueryProvider<ZincUser>>()
        .AddTransient<IQueryProvider<Project>, MongoQueryProvider<Project>>()
        ;
}
