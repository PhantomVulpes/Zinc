using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;

namespace Vulpes.Zinc.Domain.Commands;
public record CreateTicketCommand(Guid ProjectKey, string Title, string Description, Guid CreatorKey) : Command;
public class CreateTicketCommandHandler : CommandHandler<CreateTicketCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IQueryProvider<Project> queryProvider;

    public CreateTicketCommandHandler(IDataRepository<Ticket> ticketRepository, IQueryProvider<Project> queryProvider)
    {
        this.ticketRepository = ticketRepository;
        this.queryProvider = queryProvider;
    }

    protected override async Task InternalExecuteAsync(CreateTicketCommand command)
    {
        var project = (await queryProvider.BeginQueryAsync()).Where(project => project.Key == command.ProjectKey).ConcealFirst().RevealOrHoax();
        var ticket = Ticket.Default with
        {
            Title = command.Title,
            Description = command.Description,
            ReporterKey = command.CreatorKey,
            ProjectKey = project.Key,
            Status = project.DefaultTicketStatus,
        };

        ticket.ValidateOrThrow();

        await ticketRepository.InsertAsync(ticket);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(CreateTicketCommand command)
    {
        var allowedProjects = await GetProjectsForUserHandler.RetrieveProjects(queryProvider, command.CreatorKey);

        if (allowedProjects.Any(project => project.Key == command.ProjectKey))
        {
            return AccessResult.Success();
        }
        else
        {
            return AccessResult.Fail("User can not access the current project.");
        }
    }
}