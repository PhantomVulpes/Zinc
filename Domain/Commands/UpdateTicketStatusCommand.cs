using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record UpdateTicketStatusCommand(Guid TicketKey, TicketStatus NewStatus, Guid UserKey) : Command;
public class UpdateTicketStatusCommandHandler : CommandHandler<UpdateTicketStatusCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IDataRepository<Project> projectRepository;

    public UpdateTicketStatusCommandHandler(IDataRepository<Ticket> ticketRepository, IDataRepository<Project> projectRepository)
    {
        this.ticketRepository = ticketRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(UpdateTicketStatusCommand command)
    {
        var originalTicket = await ticketRepository.GetAsync(command.TicketKey);
        var updatedTicket = originalTicket with
        {
            Status = command.NewStatus
        };

        updatedTicket.ValidateOrThrow();

        await ticketRepository.SaveAsync(originalTicket.EditingToken, updatedTicket);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(UpdateTicketStatusCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        var project = await projectRepository.GetAsync(ticket.ProjectKey);

        // If the user can access the project, they can update the ticket status.
        if (project.AllowedUserKeys.Contains(command.UserKey) || project.CreatorKey == command.UserKey)
        {
            return AccessResult.Success();
        }
        else
        {
            return AccessResult.Fail($"User {command.UserKey} does not have access to the parent project {project.ToLogName()}.");
        }
    }
}