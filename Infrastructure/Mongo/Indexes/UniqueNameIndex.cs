using MongoDB.Driver;
using Vulpes.Electrum.Domain.Mongo;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Infrastructure.Mongo.Indexes;

public class UniqueNameIndex : MongoIndexDefinition<RegisteredUser>
{
    public override string Name => "name_unique";

    public UniqueNameIndex(IMongoProvider mongoProvider) : base(mongoProvider) { }

    protected override async Task CreateIndexInternalAsync()
    {
        var collection = GetCollection();

        var index = new CreateIndexModel<RegisteredUser>(
            Builders<RegisteredUser>.IndexKeys
                    .Ascending(user => user.Username),
                new CreateIndexOptions()
                {
                    Unique = true,
                    Name = Name
                }
            );

        await collection.Indexes.CreateOneAsync(index);
    }
}