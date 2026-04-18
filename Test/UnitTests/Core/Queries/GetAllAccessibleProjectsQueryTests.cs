using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Queries;

[TestClass]
public class GetAllAccessibleProjectsQueryTests
{
    private readonly TestQueryProvider<Project> testProjectProvider;
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly GetAllAccessibleProjectsQueryHandler underTest;

    public GetAllAccessibleProjectsQueryTests()
    {
        testProjectProvider = new();
        testUserRepository = new();

        var user = RegisteredUser.Default;
        var projects = new List<Project>()
        {
            Project.Default with
            {
                CreatorKey = user.Key,
                AllowedUserKeys = [user.Key]
            },
            Project.Default with
            {
                CreatorKey = Guid.NewGuid(),
                AllowedUserKeys = [Guid.NewGuid()]
            }
        };

        testProjectProvider.Response = projects;
        testUserRepository.Entries.Add(user.Key, user);

        underTest = new(testProjectProvider, testUserRepository);
    }

    [TestMethod]
    public async Task NonAdminUser_OnlySeesAccessibleProjects()
    {
        var user = testUserRepository.Entries.First().Value;
        var query = new GetAllAccessibleProjectsQuery(user.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.All(p => p.AllowedUserKeys.Contains(user.Key)));
    }

    [TestMethod]
    public async Task AdminUser_SeesAllProjects()
    {
        var adminUser = RegisteredUser.Default with { Role = Role.Admin };
        testUserRepository.Entries.Add(adminUser.Key, adminUser);
        var query = new GetAllAccessibleProjectsQuery(adminUser.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task UserWithNoAccessibleProjects_ReturnsEmptyResult()
    {
        var userWithNoAccess = RegisteredUser.Default with { Key = Guid.NewGuid() };
        testUserRepository.Entries.Add(userWithNoAccess.Key, userWithNoAccess);
        var query = new GetAllAccessibleProjectsQuery(userWithNoAccess.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(0, result.Count());
    }
}
