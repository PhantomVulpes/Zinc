using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Data;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Core.Models;

namespace Vulpes.Zinc.Core.Commands;

public record LogInCommand(RegisteredUser User) : Command;
public class LogInCommandHandler : CommandHandler<LogInCommand>
{
    private readonly IModelRepository<RegisteredUser> userRepository;

    public LogInCommandHandler(IModelRepository<RegisteredUser> userRepository)
    {
        this.userRepository = userRepository;
    }

    protected override async Task InternalExecuteAsync(LogInCommand command)
    {
        var updatedUser = command.User with
        {
            LastLoginDate = DateTime.UtcNow
        };

        await userRepository.SaveAsync(updatedUser.PrepareForSave());
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(LogInCommand command) => AccessResult.Success().FromResult();
}