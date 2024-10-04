using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record UpdateTicketDescriptionCommand(Guid TicketKey, string Description, Guid UserKey) : Command;
public class UpdateTicketDescriptionCommandHandler : CommandHandler<UpdateTicketDescriptionCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IDataRepository<Project> projectRepository;

    public UpdateTicketDescriptionCommandHandler(IDataRepository<Ticket> ticketRepository, IDataRepository<Project> projectRepository)
    {
        this.ticketRepository = ticketRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(UpdateTicketDescriptionCommand command)
    {
        var originalTicket = await ticketRepository.GetAsync(command.TicketKey);
        var updatedTicket = originalTicket with
        {
            Description = command.Description
        };

        updatedTicket.ValidateOrThrow();

        await ticketRepository.SaveAsync(originalTicket.EditingToken, updatedTicket);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(UpdateTicketDescriptionCommand command)
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