using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetTicketByKey(Guid TicketKey) : Query;
public class GetTicketByKeyHandler : QueryHandler<GetTicketByKey, Ticket>
{
    private readonly IDataRepository<Ticket> repository;

    public GetTicketByKeyHandler(IDataRepository<Ticket> repository)
    {
        this.repository = repository;
    }

    protected override async Task<Ticket> InternalRequestAsync(GetTicketByKey query)
    {
        var ticket = await repository.GetAsync(query.TicketKey);
        return ticket ?? Ticket.Empty;
    }
}