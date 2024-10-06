using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record AddTicketCommentCommand(string Comment, Guid TicketKey, Guid UserKey) : Command;
public class AddTicketCommentCommandHandler : CommandHandler<AddTicketCommentCommand>
{
    private readonly IDataRepository<Ticket> ticketRepository;
    private readonly IDataRepository<Project> projectRepository;

    public AddTicketCommentCommandHandler(IDataRepository<Ticket> ticketRepository, IDataRepository<Project> projectRepository)
    {
        this.ticketRepository = ticketRepository;
        this.projectRepository = projectRepository;
    }

    protected async override Task InternalExecuteAsync(AddTicketCommentCommand command)
    {
        var comment = Comment.Default with
        {
            Author = command.UserKey,
            Value = command.Comment,
        };

        var ticket = (await ticketRepository.GetAsync(command.TicketKey)).AddComment(comment);
        ticket.ValidateOrThrow();

        await ticketRepository.SaveAsync(ticket.EditingToken, ticket);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(AddTicketCommentCommand command)
    {
        var ticket = await ticketRepository.GetAsync(command.TicketKey);
        var project = await projectRepository.GetAsync(ticket.ProjectKey);

        if (project.CreatorKey == command.UserKey || project.AllowedUserKeys.Contains(command.UserKey))
        {
            return AccessResult.Success();
        }

        return AccessResult.Fail($"User {command.UserKey} does not have access to {project.ToLogName()} and cannot add a comment to {ticket.ToLogName()}.");
    }
}