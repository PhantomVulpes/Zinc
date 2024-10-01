using Vulpes.Electrum.Core.Domain.Querying;
using Vulpes.Zinc.Domain.Data;
using Vulpes.Zinc.Domain.Models;

namespace Vulpes.Zinc.Domain.Queries;
public record GetProjectByShorthand(string Shorthand) : Query;
public class GetProjectByShorthandHandler : QueryHandler<GetProjectByShorthand, Project>
{
    private readonly IQueryProvider<Project> queryProvider;

    public GetProjectByShorthandHandler(IQueryProvider<Project> queryProvider)
    {
        this.queryProvider = queryProvider;
    }

    protected override async Task<Project> InternalRequestAsync(GetProjectByShorthand query)
    {
        // TODO: Make sure relics are used to return an empty project. Don't think I have the correct extension in Electrum yet.
        var result = (await queryProvider.BeginQueryAsync()).FirstOrDefault(project => project.Shorthand == query.Shorthand.ToUpper());
        return result;
    }
}