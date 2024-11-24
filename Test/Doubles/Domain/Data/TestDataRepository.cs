using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Test.Doubles.Domain.Data;
public class TestDataRepository<TAggregateRoot> : IDataRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    public ZincUser PreparedUser { get; set; } = ZincUser.Empty;

    public List<TAggregateRoot> Inserted { get; } = [];
    public List<TAggregateRoot> Deleted { get; } = [];
    public List<TAggregateRoot> Saved { get; } = [];

    public Dictionary<Guid, TAggregateRoot> Models { get; } = [];

    public Task<TAggregateRoot> GetAsync(Guid key) => Models[key].FromResult();
    public Task InsertAsync(TAggregateRoot record)
    {
        Inserted.Add(record);
        Models.Add(record.Key, record);

        return Task.CompletedTask;
    }

    public Task SaveAsync(string editingToken, TAggregateRoot record)
    {
        Saved.Add(record);
        Models[record.Key] = record;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(TAggregateRoot record)
    {
        Deleted.Add(record);
        Models.Remove(record.Key);

        return Task.CompletedTask;
    }
}
