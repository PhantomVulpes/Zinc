using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Data;
public interface IDataRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    Task<TAggregateRoot> GetAsync(Guid key);
    Task SaveAsync(string editingToken, TAggregateRoot record);
    Task DeleteAsync(TAggregateRoot record);
    Task InsertAsync(TAggregateRoot record);
}
