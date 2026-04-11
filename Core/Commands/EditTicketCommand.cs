using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Extensions;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Commands;

public record EditTicketCommand(
    Guid ProjectKey,
    int TicketIndex,
    string Title,
    string Description,
    IEnumerable<string> Labels,
    TicketStatus Status,
    Guid UserKey) : Command;

public class EditTicketCommandHandler : CommandHandler<EditTicketCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<Project> projectRepository;

    public EditTicketCommandHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<Project> projectRepository)
    {
        this.userRepository = userRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(EditTicketCommand command)
    {
        var project = await projectRepository.GetAsync(command.ProjectKey);
        var ticket = project.Tickets.Single(ticket => ticket.Index == command.TicketIndex);

        var updatedTicket = ticket with
        {
            Title = command.Title,
            Description = command.Description,
            Labels = command.Labels,
            Status = command.Status
        };

        var updatedProject = project.WithUpdatedTicket(updatedTicket.Validate());

        await projectRepository.SaveAsync(updatedProject.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(EditTicketCommand command)
    {
        var user = await userRepository.GetAsync(command.UserKey);

        if (user.Role == Role.Admin)
        {
            return AccessResult.Success();
        }

        var project = await projectRepository.GetAsync(command.ProjectKey);

        if (!project.AllowedUserKeys.Contains(command.UserKey))
        {
            return AccessResult.Fail($"User {user.ToLogName()} does not have access to {project.ToLogName()}.");
        }

        return AccessResult.Success();
    }
}
