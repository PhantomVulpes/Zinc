using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Domain.Queries;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Queries;

[TestClass]
public class GetProjectsForUserTests
{
    private readonly TestQueryProvider<Project> testQueryProvider;

    private readonly GetProjectsForUserHandler underTest;

    public GetProjectsForUserTests()
    {
        testQueryProvider = new();

        underTest = new(testQueryProvider);
    }

    [TestMethod]
    public async Task RetrieveProjects_ReturnsOwnedProjects()
    {
        var userKey = Guid.NewGuid();
        var project1 = Project.Default with { Name = "Project 1", CreatorKey = userKey };
        var project2 = Project.Default with { Name = "Project 2", CreatorKey = Guid.NewGuid() };
        var project3 = Project.Default with { Name = "Project 3", CreatorKey = userKey };

        testQueryProvider.Response.AddRange([project1, project2, project3]);

        var result = await GetProjectsForUserHandler.RetrieveProjects(testQueryProvider, userKey);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Contains(project1));
        Assert.IsFalse(result.Contains(project2));
        Assert.IsTrue(result.Contains(project3));
    }

    [TestMethod]
    public async Task RetrieveProjects_ReturnsAllowedProjects()
    {
        var userKey = Guid.NewGuid();
        var project1 = Project.Default with { Name = "Project 1", AllowedUserKeys = [userKey] };
        var project2 = Project.Default with { Name = "Project 2", AllowedUserKeys = [Guid.NewGuid()] };
        var project3 = Project.Default with { Name = "Project 3", AllowedUserKeys = [userKey] };

        testQueryProvider.Response.AddRange([project1, project2, project3]);

        var result = await GetProjectsForUserHandler.RetrieveProjects(testQueryProvider, userKey);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Contains(project1));
        Assert.IsFalse(result.Contains(project2));
        Assert.IsTrue(result.Contains(project3));
    }
}
