using MongoDB.Bson.Serialization.Conventions;

namespace Vulpes.Zinc.External.Mongo;
public static class MongoConfigurator
{
    public static void Configure()
    {
        var pack = new ConventionPack()
        {
            new ConfigureToStringConvention(typeInfo => typeInfo.IsEnum),
            new ConfigureToStringConvention(typeInfo => typeInfo == typeof(Guid)),
        };

        ConventionRegistry.Register("Global MongoDB Conventions", pack, t => true);
    }
}