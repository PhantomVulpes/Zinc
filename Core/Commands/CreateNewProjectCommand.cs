using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Commands;

public record CreateNewProjectCommand(Guid ProjectKey, string ProjectName, string ProjectShorthand, string ProjectDescription, Guid AuthorizedUserKey) : Command;
public class CreateNewProjectCommandHandler : CommandHandler<CreateNewProjectCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;
    private readonly IModelRepository<Project> projectRepository;

    public CreateNewProjectCommandHandler(IModelRepository<RegisteredUser> userRepository, IModelRepository<Project> projectRepository)
    {
        this.userRepository = userRepository;
        this.projectRepository = projectRepository;
    }

    protected override async Task InternalExecuteAsync(CreateNewProjectCommand command)
    {
        // TODO: Index to verify only one project with the given name and shorthand exists.

        var newProject = Project.Default with
        {
            Key = command.ProjectKey,
            Name = command.ProjectName,
            Shorthand = command.ProjectShorthand.ToUpper(),
            Description = command.ProjectDescription,
            CreatorKey = command.AuthorizedUserKey,
            AllowedUserKeys = [command.AuthorizedUserKey],
        };

        await projectRepository.InsertAsync(newProject.PrepareForInsert());
    }

    // Any user can create a new project provided they are logged in.
    protected override async Task<AccessResult> InternalValidateAccessAsync(CreateNewProjectCommand command)
    {
        // TODO: Electrum should be updated to always GetPerhaps instead. Get should be an extension.
        _ = await userRepository.GetAsync(command.AuthorizedUserKey);

        // The lookup didn't fail, the user is real.
        return AccessResult.Success();
    }
}