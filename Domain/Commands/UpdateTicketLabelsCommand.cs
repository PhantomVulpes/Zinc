using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record UpdateTicketLabelsCommand(IEnumerable<string> NewLabels, Guid TicketKey, Guid ExecutingUserKey) : Command;
public class UpdateTicketLabelsCommandHandler : CommandHandler<UpdateTicketLabelsCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IDataRepository<Project> projectRepository;

    public UpdateTicketLabelsCommandHandler(IDataRepository<Ticket> ticketRepository, IDataRepository<Project> projectRepository)
    {
        this.ticketRepository = ticketRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(UpdateTicketLabelsCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        var updatedTicket = ticket with
        {
            Labels = command.NewLabels
        };

        await ticketRepository.SaveAsync(ticket.EditingToken, updatedTicket);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(UpdateTicketLabelsCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        var project = await projectRepository.GetAsync(ticket.ProjectKey);

        return project.UserIsAllowed(command.ExecutingUserKey);
    }
}