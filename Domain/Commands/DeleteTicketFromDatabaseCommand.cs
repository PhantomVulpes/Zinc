using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record DeleteTicketFromDatabaseCommand(Guid TicketKey, Guid UserKey) : Command;
public class DeleteTicketFromDatabaseCommandHandler : CommandHandler<DeleteTicketFromDatabaseCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IDataRepository<Project> projectRepository;

    public DeleteTicketFromDatabaseCommandHandler(IDataRepository<Ticket> ticketRepository, IDataRepository<Project> projectRepository)
    {
        this.ticketRepository = ticketRepository;
        this.projectRepository = projectRepository;
    }

    protected async override Task InternalExecuteAsync(DeleteTicketFromDatabaseCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        await ticketRepository.DeleteAsync(ticket);
    }

    protected async override Task<AccessResult> InternalValidateAccessAsync(DeleteTicketFromDatabaseCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        var project = await projectRepository.GetAsync(ticket.ProjectKey);

        if (!project.UserIsAllowed(command.UserKey))
        {
            return AccessResult.Fail($"User {command.UserKey} does not have access to project {project.ToLogName()}.");
        }

        if (ticket.Status != TicketStatus.Cancelled)
        {
            return AccessResult.Fail($"Ticket {ticket.ToLogName()} is not in a state that can be deleted.");
        }

        // If we've made it this far, then we passed.
        return AccessResult.Success();
    }
}
