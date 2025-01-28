using System;
using Vulpes.Electrum.Domain.Commanding;
using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Security;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Security;

namespace Vulpes.Zinc.Domain.Commands;

public record InitializeAllIndexesCommand(ZincUser CurrentUser) : Command;
public class InitializeAllIndexesCommandHandler : CommandHandler<InitializeAllIndexesCommand>
{
    private readonly IEnumerable<IIndexDefinition> indexDefinitions;

    public InitializeAllIndexesCommandHandler(IEnumerable<IIndexDefinition> indexDefinitions)
    {
        this.indexDefinitions = indexDefinitions;
    }

    protected override Task<AccessResult> InternalValidateAccessAsync(InitializeAllIndexesCommand command)
    {
        if (command.CurrentUser.Role == Role.Admin)
        {
            return AccessResult.Success().FromResult();
        }
        
        return AccessResult.Fail($"{command.CurrentUser} is not authorized to initialize indexes. {Role.Admin} level is required.").FromResult();
    }

    protected override async Task InternalExecuteAsync(InitializeAllIndexesCommand command)
    {
        foreach (var indexDefinition in indexDefinitions)
        {
            if (await indexDefinition.ExistsAsync())
            {
                continue;
            }

            await indexDefinition.CreateIndexAsync();
        }
    }

}
