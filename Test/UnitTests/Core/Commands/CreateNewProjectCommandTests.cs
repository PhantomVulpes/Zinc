using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class CreateNewProjectCommandTests
{
    private readonly CreateNewProjectCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    public CreateNewProjectCommandTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        underTest = new(testUserRepository, testProjectRepository);
    }

    private CreateNewProjectCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        var project = Project.Default;

        testUserRepository.AddEntryForTest(user);

        return new(project.Key, "Test Project", "TP", "This is a test project", user.Key);
    }

    [TestMethod]
    public async Task ProjectIsInsertedToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testProjectRepository.InsertedEntries.Count);
    }

    [TestMethod]
    public async Task ProjectPropertiesAreSetCorrectly()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var insertedProject = testProjectRepository.InsertedEntries.First();
        Assert.AreEqual(command.ProjectKey, insertedProject.Key);
        Assert.AreEqual(command.ProjectName, insertedProject.Name);
        Assert.AreEqual(command.ProjectDescription, insertedProject.Description);
        Assert.AreEqual(command.AuthorizedUserKey, insertedProject.CreatorKey);
    }

    [TestMethod]
    public async Task ProjectShorthandIsUppercased()
    {
        var command = PrepareCommand() with { ProjectShorthand = "lowercase" };

        await underTest.ExecuteAsync(command);

        var insertedProject = testProjectRepository.InsertedEntries.First();
        Assert.AreEqual("LOWERCASE", insertedProject.Shorthand);
        Assert.AreNotEqual(command.ProjectShorthand, insertedProject.Shorthand);
    }

    [TestMethod]
    public async Task CreatorIsAddedToAllowedUsers()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var insertedProject = testProjectRepository.InsertedEntries.First();
        Assert.IsTrue(insertedProject.AllowedUserKeys.Contains(command.AuthorizedUserKey));
        Assert.AreEqual(1, insertedProject.AllowedUserKeys.Count());
    }

    [TestMethod]
    public async Task ProjectStatusIsSetToOpen()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        var insertedProject = testProjectRepository.InsertedEntries.First();
        Assert.AreEqual(ProjectStatus.Open, insertedProject.Status);
    }

    [TestMethod]
    public async Task ProjectDefaultTicketStatusIsSetToInReview()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        var insertedProject = testProjectRepository.InsertedEntries.First();
        Assert.AreEqual(TicketStatus.InReview, insertedProject.DefaultTicketStatus);
    }

    [TestMethod]
    public async Task AnyLoggedInUserCanCreateProject()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.InsertedEntries.Count);
    }
}
