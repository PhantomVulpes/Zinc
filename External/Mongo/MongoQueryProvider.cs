using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Domain.Data;

namespace Vulpes.Zinc.External.Mongo;

public class MongoQueryProvider<TResponse> : IQueryProvider<TResponse>
{
    private readonly IMongoProvider mongoProvider;

    public MongoQueryProvider(IMongoProvider mongoProvider)
    {
        this.mongoProvider = mongoProvider;
    }

    public Task<IQueryable<TResponse>> BeginQueryAsync()
    {
        var result = mongoProvider.GetQuery<TResponse>(CqrsType.Query).AsQueryable();
        return result.FromResult();
    }
}