using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Queries;

[TestClass]
public class GetProjectByKeyQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    private readonly GetProjectByKeyQueryHandler underTest;

    private readonly RegisteredUser regularUser;
    private readonly RegisteredUser adminUser;
    private readonly RegisteredUser unauthorizedUser;
    private readonly Project accessibleProject;
    private readonly Project inaccessibleProject;

    public GetProjectByKeyQueryTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        regularUser = RegisteredUser.Default;
        adminUser = RegisteredUser.Default with { Key = Guid.NewGuid(), Role = Role.Admin };
        unauthorizedUser = RegisteredUser.Default with { Key = Guid.NewGuid() };

        accessibleProject = Project.Default with
        {
            Key = Guid.NewGuid(),
            CreatorKey = regularUser.Key,
            AllowedUserKeys = [regularUser.Key]
        };

        inaccessibleProject = Project.Default with
        {
            Key = Guid.NewGuid(),
            CreatorKey = Guid.NewGuid(),
            AllowedUserKeys = [Guid.NewGuid()]
        };

        testUserRepository.Entries.Add(regularUser.Key, regularUser);
        testUserRepository.Entries.Add(adminUser.Key, adminUser);
        testUserRepository.Entries.Add(unauthorizedUser.Key, unauthorizedUser);

        testProjectRepository.Entries.Add(accessibleProject.Key, accessibleProject);
        testProjectRepository.Entries.Add(inaccessibleProject.Key, inaccessibleProject);

        underTest = new(testUserRepository, testProjectRepository);
    }

    [TestMethod]
    public async Task NonAdminUser_CanGetAccessibleProject()
    {
        var query = new GetProjectByKeyQuery(accessibleProject.Key, regularUser.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(accessibleProject.Key, result.Key);
    }

    [TestMethod]
    public async Task NonAdminUser_CannotGetInaccessibleProject()
    {
        var query = new GetProjectByKeyQuery(inaccessibleProject.Key, unauthorizedUser.Key);

        var access = await underTest.ValidateAccessAsync(query);

        Assert.IsFalse(access);
    }

    [TestMethod]
    public async Task AdminUser_CanGetAnyProject()
    {
        var query = new GetProjectByKeyQuery(inaccessibleProject.Key, adminUser.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(inaccessibleProject.Key, result.Key);
    }
}
