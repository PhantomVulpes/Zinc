using Vulpes.Electrum.Core.Domain.Exceptions;
using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetTicketStatsByProject(Guid ProjectKey, Guid UserKey) : Query;
public class GetTicketStatsByProjectHandler : QueryHandler<GetTicketStatsByProject, Dictionary<TicketStatus, int>>
{
    private readonly IDataRepository<Project> projectRepository;
    private readonly IQueryProvider<Ticket> ticketQueryProvider;

    public GetTicketStatsByProjectHandler(IDataRepository<Project> projectRepository, IQueryProvider<Ticket> ticketQueryProvider)
    {
        this.projectRepository = projectRepository;
        this.ticketQueryProvider = ticketQueryProvider;
    }

    protected override async Task<Dictionary<TicketStatus, int>> InternalRequestAsync(GetTicketStatsByProject query)
    {
        var project = await projectRepository.GetAsync(query.ProjectKey);
        if (!project.UserIsAllowed(query.UserKey))
        {
            throw new AccessDeniedException(AccessResult.Fail($"User {query.UserKey} does not have access to project {project.ToLogName()}."));
        }

        var results = new Dictionary<TicketStatus, int>();
        foreach (var status in Enum.GetValues<TicketStatus>())
        {
            var ticketsInProject = (await ticketQueryProvider.BeginQueryAsync()).Where(ticket => ticket.ProjectKey == project.Key && ticket.Status == status).Count();
            results.Add(status, ticketsInProject);
        }

        return results;
    }
}
