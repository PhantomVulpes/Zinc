using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class LogInCommandTests
{
    private readonly LogInCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    public LogInCommandTests()
    {
        testUserRepository = new();

        underTest = new(testUserRepository);
    }

    private LogInCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        testUserRepository.AddEntryForTest(user);

        return new(user);
    }

    [TestMethod]
    public async Task UserSavesToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testUserRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task LastLoginDateIsUpdated()
    {
        var command = PrepareCommand();
        var initialUser = command.User;
        var initialLastLoginDate = initialUser.LastLoginDate;

        await underTest.ExecuteAsync(command);

        var updatedUser = testUserRepository.SavedEntries.First();

        Assert.AreNotEqual(initialLastLoginDate, updatedUser.LastLoginDate);
        Assert.AreNotEqual(DateTimeOffset.MinValue, updatedUser.LastLoginDate);
    }

    [TestMethod]
    public async Task OtherUserPropertiesAreUnchanged()
    {
        var command = PrepareCommand();
        var initialUser = command.User;

        await underTest.ExecuteAsync(command);

        var updatedUser = testUserRepository.SavedEntries.First();

        Assert.AreEqual(initialUser.Key, updatedUser.Key);
        Assert.AreEqual(initialUser.FirstName, updatedUser.FirstName);
        Assert.AreEqual(initialUser.LastName, updatedUser.LastName);
        Assert.AreEqual(initialUser.Username, updatedUser.Username);
        Assert.AreEqual(initialUser.PasswordHash, updatedUser.PasswordHash);
        Assert.AreEqual(initialUser.Role, updatedUser.Role);
    }
}
