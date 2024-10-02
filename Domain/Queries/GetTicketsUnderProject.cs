using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetTicketsUnderProject(Guid ProjectKey) : Query();
public class GetTicketsUnderProjectHandler : QueryHandler<GetTicketsUnderProject, IEnumerable<Ticket>>
{
    private readonly IQueryProvider<Ticket> queryProvider;
    public GetTicketsUnderProjectHandler(IQueryProvider<Ticket> queryProvider)
    {
        this.queryProvider = queryProvider;
    }

    protected override async Task<IEnumerable<Ticket>> InternalRequestAsync(GetTicketsUnderProject query)
    {
        var tickets = (await queryProvider.BeginQueryAsync()).Where(ticket => ticket.ProjectKey == query.ProjectKey);
        return tickets;
    }
}