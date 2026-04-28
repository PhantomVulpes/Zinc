using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class EditProjectCommandTests
{
    private readonly EditProjectCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    public EditProjectCommandTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        underTest = new(testProjectRepository, testUserRepository);
    }

    private EditProjectCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [user.Key] };

        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        return new(
            project.Key,
            user.Key,
            "Updated Project Name",
            "Updated description",
            TicketStatus.InProgress,
            [user.Key],
            ProjectStatus.Archived,
            ["label1", "label2"]);
    }

    [TestMethod]
    public async Task ProjectSavesToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task ValuesAreUpdated()
    {
        var command = PrepareCommand();

        var initialProject = testProjectRepository.Entries.First().Value;
        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();

        Assert.AreNotEqual(initialProject.Name, updatedProject.Name);
        Assert.AreEqual(command.ProjectName, updatedProject.Name);
    }

    [TestMethod]
    public async Task ProjectKeyIsUnchanged()
    {
        var command = PrepareCommand();

        var initialProject = testProjectRepository.Entries.First().Value;
        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();

        Assert.AreEqual(initialProject.Key, updatedProject.Key);
        Assert.AreEqual(command.ProjectKey, updatedProject.Key);
    }

    [TestMethod]
    public async Task ProjectShorthandIsUnchanged()
    {
        var command = PrepareCommand();

        var initialProject = testProjectRepository.Entries.First().Value;
        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();

        Assert.AreEqual(initialProject.Shorthand, updatedProject.Shorthand);
    }

    [TestMethod]
    public async Task ProjectCreatorKeyIsUnchanged()
    {
        var command = PrepareCommand();

        var initialProject = testProjectRepository.Entries.First().Value;
        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();

        Assert.AreEqual(initialProject.CreatorKey, updatedProject.CreatorKey);
    }

    [TestMethod]
    public async Task ProjectTicketsAreUnchanged()
    {
        var user = RegisteredUser.Default;
        var ticket1 = Ticket.Default(1);
        var ticket2 = Ticket.Default(2);
        var project = Project.Default with
        {
            AllowedUserKeys = [user.Key],
            Tickets = [ticket1, ticket2]
        };

        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditProjectCommand(
            project.Key,
            user.Key,
            "Updated Name",
            "Updated Description",
            TicketStatus.InProgress,
            [user.Key],
            ProjectStatus.Open,
            ["label"]);

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();

        Assert.AreEqual(2, updatedProject.Tickets.Count());
        CollectionAssert.AreEquivalent(project.Tickets.ToList(), updatedProject.Tickets.ToList());
    }

    [TestMethod]
    public async Task AdminCanEditAnyProject()
    {
        var admin = RegisteredUser.Default with { Role = Role.Admin };
        var regularUser = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [regularUser.Key] };

        testUserRepository.AddEntryForTest(admin);
        testUserRepository.AddEntryForTest(regularUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditProjectCommand(
            project.Key,
            admin.Key,
            "Admin Edit",
            "Description",
            TicketStatus.InReview,
            [regularUser.Key],
            ProjectStatus.Open,
            []);

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task AuthorizedUserCanEditProject()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task UnauthorizedUserCannotEditProject()
    {
        var authorizedUser = RegisteredUser.Default;
        var unauthorizedUser = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [authorizedUser.Key] };

        testUserRepository.AddEntryForTest(authorizedUser);
        testUserRepository.AddEntryForTest(unauthorizedUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditProjectCommand(
            project.Key,
            unauthorizedUser.Key,
            "Unauthorized Edit",
            "Description",
            TicketStatus.InReview,
            [authorizedUser.Key],
            ProjectStatus.Open,
            []);

        await Assert.ThrowsExceptionAsync<AccessDeniedException>(
            async () => await underTest.ExecuteAsync(command));

        Assert.AreEqual(0, testProjectRepository.SavedEntries.Count);
    }
}
