using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Vulpes.Zinc.External.Mongo;
public interface IMongoProvider
{
    IMongoDatabase GetDatabase(CqrsType cqrsType);
    IMongoCollection<TCollection> GetCollection<TCollection>(CqrsType cqrsType);
    IMongoQueryable<TCollection> GetQuery<TCollection>(CqrsType cqrsType);
    Task<IEnumerable<TCollection>> ExecuteQueryAsync<TCollection>(string queryDocument);
}