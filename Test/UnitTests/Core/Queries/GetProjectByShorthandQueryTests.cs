using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Queries;

[TestClass]
public class GetProjectByShorthandQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestQueryProvider<Project> testProjectQueryProvider;

    private readonly GetProjectByShorthandQueryHandler underTest;

    private readonly RegisteredUser regularUser;
    private readonly RegisteredUser adminUser;
    private readonly RegisteredUser unauthorizedUser;
    private readonly Project accessibleProject;
    private readonly Project inaccessibleProject;

    public GetProjectByShorthandQueryTests()
    {
        testUserRepository = new();
        testProjectQueryProvider = new();

        regularUser = RegisteredUser.Default;
        adminUser = RegisteredUser.Default with { Key = Guid.NewGuid(), Role = Role.Admin };
        unauthorizedUser = RegisteredUser.Default with { Key = Guid.NewGuid() };

        accessibleProject = Project.Default with
        {
            Key = Guid.NewGuid(),
            Shorthand = "ACCESSIBLE",
            CreatorKey = regularUser.Key,
            AllowedUserKeys = [regularUser.Key]
        };

        inaccessibleProject = Project.Default with
        {
            Key = Guid.NewGuid(),
            Shorthand = "INACCESSIBLE",
            CreatorKey = Guid.NewGuid(),
            AllowedUserKeys = [Guid.NewGuid()]
        };

        testUserRepository.Entries.Add(regularUser.Key, regularUser);
        testUserRepository.Entries.Add(adminUser.Key, adminUser);
        testUserRepository.Entries.Add(unauthorizedUser.Key, unauthorizedUser);

        testProjectQueryProvider.Response = [accessibleProject, inaccessibleProject];

        underTest = new(testUserRepository, testProjectQueryProvider);
    }

    [TestMethod]
    public async Task NonAdminUser_CanGetAccessibleProjectByShorthand()
    {
        var query = new GetProjectByShorthandQuery(accessibleProject.Shorthand, regularUser.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(accessibleProject.Shorthand, result.Shorthand);
    }

    [TestMethod]
    public async Task NonAdminUser_CannotGetInaccessibleProjectByShorthand()
    {
        var query = new GetProjectByShorthandQuery(inaccessibleProject.Shorthand, unauthorizedUser.Key);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AdminUser_CanGetAnyProjectByShorthand()
    {
        var query = new GetProjectByShorthandQuery(inaccessibleProject.Shorthand, adminUser.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(inaccessibleProject.Shorthand, result.Shorthand);
    }
}
