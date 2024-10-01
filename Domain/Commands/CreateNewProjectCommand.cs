using Vulpes.Electrum.Core.Domain.Commanding;
using Vulpes.Electrum.Core.Domain.Extensions;
using Vulpes.Electrum.Core.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Commands;
public record CreateNewProjectCommand(string ProjectName, string ProjectShorthand, string ProjectDescription, IEnumerable<string> AllowedEmails, Guid UserKey) : Command;
public class CreateNewProjectCommandHandler : CommandHandler<CreateNewProjectCommand>
{
    private readonly IDataRepository<Project> projectRepository;

    public CreateNewProjectCommandHandler(IDataRepository<Project> projectRepository)
    {
        this.projectRepository = projectRepository;
    }

    protected async override Task InternalExecuteAsync(CreateNewProjectCommand command)
    {
        // TODO: Ensure only one project with the same name exists with indexes.
        // TODO: Get user keys by email.

        var project = Project.Default with
        {
            Name = command.ProjectName,
            Description = command.ProjectDescription,
            AllowedUserKeys = [command.UserKey],
            CreatorKey = command.UserKey,
            Shorthand = command.ProjectShorthand.ToUpper(),
        };

        project.ValidateOrThrow();

        await projectRepository.InsertAsync(project);
    }

    // Command allows any user to access it.
    protected override Task<AccessResult> InternalValidateAccessAsync(CreateNewProjectCommand command) => AccessResult.Success().FromResult();
}
