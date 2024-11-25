using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Domain.Data;

namespace Vulpes.Zinc.Test.Doubles.Domain.Data;
public class TestQueryProvider<TResponse> : IQueryProvider<TResponse>
{
    public List<TResponse> Response { get; } = [];

    public Task<IQueryable<TResponse>> BeginQueryAsync() => Response.AsQueryable().FromResult();
}
