using Vulpes.Electrum.Domain.Extensions;
using Vulpes.Electrum.Domain.Querying;
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
        var result = (await queryProvider.BeginQueryAsync()).FirstOrPerhaps(project => project.Shorthand == query.Shorthand.ToUpper());
        return result.ElseThrow();
    }
}