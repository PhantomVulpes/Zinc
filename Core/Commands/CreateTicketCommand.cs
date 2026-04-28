using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Extensions;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Commands;

public record CreateTicketCommand(Guid ProjectKey, string Title, string Description, IEnumerable<string> Labels, Guid CreatorKey) : Command;
public class CreateTicketCommandHandler : CommandHandler<CreateTicketCommand>
{
    private readonly IModelRepository<Project> projectRepository;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public CreateTicketCommandHandler(IModelRepository<Project> projectRepository, IModelRepository<RegisteredUser> userRepository)
    {
        this.projectRepository = projectRepository;
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(CreateTicketCommand command)
    {
        var project = await projectRepository.GetAsync(command.ProjectKey);
        var lastTicketInOrder = project.Tickets.OrderBy(ticket => ticket.Index).LastOrPerhaps();
        var nextIndex = lastTicketInOrder.IsEmpty ? 1 : lastTicketInOrder.Get().Index + 1;

        var ticket = Ticket.Default(nextIndex) with
        {
            Title = command.Title,
            Description = command.Description,
            ReporterKey = command.CreatorKey,
            Status = project.DefaultTicketStatus,
            Labels = command.Labels
        };

        var updateProject = project.WithAddedTicket(ticket.Validate());

        await projectRepository.SaveAsync(updateProject.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(CreateTicketCommand command)
    {
        // Get the project.
        var project = await projectRepository.GetAsync(command.ProjectKey);

        // Get the user.
        var user = await userRepository.GetAsync(command.CreatorKey);

        // Does the user have access to this project?
        if (user.Role == Role.Admin || project.AllowedUserKeys.Contains(command.CreatorKey))
        {
            return AccessResult.Success();
        }
        else
        {
            return AccessResult.Fail($"{nameof(RegisteredUser)} {user.ToLogName()} can not access {nameof(Project)} {project.ToLogName()}.");
        }
    }
}