namespace Vulpes.Zinc.Domain.Data;
public interface IQueryProvider<TResponse>
{
    Task<IQueryable<TResponse>> BeginQueryAsync();
}