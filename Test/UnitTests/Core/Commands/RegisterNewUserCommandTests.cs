using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Security;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class RegisterNewUserCommandTests
{
    private readonly RegisterNewUserCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestKnoxHasher testKnoxHasher;

    public RegisterNewUserCommandTests()
    {
        testUserRepository = new();
        testKnoxHasher = new();

        underTest = new(testUserRepository, testKnoxHasher);
    }

    private RegisterNewUserCommand PrepareCommand() => PrepareCommand("test-password", true);
    private RegisterNewUserCommand PrepareCommand(string testPassword, bool grantAccess)
    {
        var user = RegisteredUser.Default with { PasswordHash = testKnoxHasher.MockedHashResult };

        testKnoxHasher.HashResults.Add((testPassword, grantAccess));

        return new(user.Key, user.FirstName, user.LastName, user.Username, testPassword);
    }

    [TestMethod]
    public async Task UserIsInsertedToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testUserRepository.InsertedEntries.Count);
    }

    [TestMethod]
    public async Task PasswordIsHashed()
    {
        var rawPassword = "my-secure-password";
        var command = PrepareCommand(rawPassword, true);

        await underTest.ExecuteAsync(command);

        var insertedUser = testUserRepository.InsertedEntries.First();
        Assert.AreEqual(testKnoxHasher.MockedHashResult, insertedUser.PasswordHash);
        Assert.AreNotEqual(rawPassword, insertedUser.PasswordHash);
    }

    [TestMethod]
    public async Task UserPropertiesAreSetCorrectly()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var insertedUser = testUserRepository.InsertedEntries.First();
        Assert.AreEqual(command.Key, insertedUser.Key);
        Assert.AreEqual(command.FirstName, insertedUser.FirstName);
        Assert.AreEqual(command.LastName, insertedUser.LastName);
        Assert.AreEqual(command.Username, insertedUser.Username);
    }

    [TestMethod]
    public async Task NewUserRoleIsSetToBasic()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        var insertedUser = testUserRepository.InsertedEntries.First();
        Assert.AreEqual(Role.Basic, insertedUser.Role);
    }
}