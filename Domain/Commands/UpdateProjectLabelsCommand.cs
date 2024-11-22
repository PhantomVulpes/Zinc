using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record UpdateProjectLabelsCommand(Guid ProjectKey, IEnumerable<string> NewLabels, Guid ExecutingUserKey) : Command;
public class UpdateProjectLabelsCommandHandler : CommandHandler<UpdateProjectLabelsCommand>
{
    private readonly IDataRepository<Project> projectRepository;

    public UpdateProjectLabelsCommandHandler(IDataRepository<Project> projectRepository)
    {
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(UpdateProjectLabelsCommand command)
    {
        var project = await projectRepository.GetAsync(command.ProjectKey);
        var updatedProject = project with
        {
            Labels = command.NewLabels
        };

        await projectRepository.SaveAsync(project.EditingToken, updatedProject);
    }

    protected override async Task<AccessResult> InternalValidateAccessAsync(UpdateProjectLabelsCommand command)
    {
        var project = await projectRepository.GetAsync(command.ProjectKey);

        return project.UserIsAllowed(command.ExecutingUserKey);
    }
}