using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;

namespace Vulpes.Zinc.Core.Commands;

public record EditProjectCommand(Guid ProjectKey, Guid UserKey, string ProjectName, string Description, TicketStatus DefaultTicketStatus, IEnumerable<Guid> AllowedUserKeys, ProjectStatus ProjectStatus, IEnumerable<string> Labels) : Command;
public class EditProjectCommandHandler : CommandHandler<EditProjectCommand>
{
    private readonly IModelRepository<Project> projectRepository;
    private readonly IModelRepository<RegisteredUser> userRepository;

    public EditProjectCommandHandler(IModelRepository<Project> projectRepository, IModelRepository<RegisteredUser> userRepository)
    {
        this.projectRepository = projectRepository;
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(EditProjectCommand command)
    {
        var project = await projectRepository.GetAsync(command.ProjectKey);

        var updatedProject = project with
        {
            Name = command.ProjectName,
            Description = command.Description,
            DefaultTicketStatus = command.DefaultTicketStatus,
            AllowedUserKeys = command.AllowedUserKeys,
            Status = command.ProjectStatus,
            Labels = command.Labels,
        };

        await projectRepository.SaveAsync(updatedProject.PrepareForSave());
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(EditProjectCommand command)
    {
        var user = await userRepository.GetAsync(command.UserKey);

        if (user.Role == Role.Admin)
        {
            return AccessResult.Success();
        }

        var project = await projectRepository.GetAsync(command.ProjectKey);

        // TODO: Ensure there is a test for the OwnerKey, not totally sure it gets added to this by default.
        if (!project.AllowedUserKeys.Contains(command.UserKey))
        {
            return AccessResult.Fail($"User {user.ToLogName()} does not have access to edit {nameof(Project)} {project.ToLogName()}.");
        }

        return AccessResult.Success();
    }
}