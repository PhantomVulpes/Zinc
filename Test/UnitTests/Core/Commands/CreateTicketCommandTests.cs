using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class CreateTicketCommandTests
{
    private readonly CreateTicketCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    public CreateTicketCommandTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        underTest = new(testProjectRepository, testUserRepository);
    }

    private CreateTicketCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [user.Key] };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        return new(project.Key, "Test Ticket", "This is a test ticket description", ["bug", "urgent"], user.Key);
    }

    [TestMethod]
    public async Task ProjectSavesToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task TicketIsAddedToProject()
    {
        var command = PrepareCommand();

        var initialProject = testProjectRepository.Entries.First().Value;
        var initialTicketCount = initialProject.Tickets.Count();

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var updatedTicketCount = updatedProject.Tickets.Count();

        Assert.AreEqual(initialTicketCount + 1, updatedTicketCount);
    }

    [TestMethod]
    public async Task TicketPropertiesAreSetCorrectly()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var addedTicket = updatedProject.Tickets.First();

        Assert.AreEqual(command.Title, addedTicket.Title);
        Assert.AreEqual(command.Description, addedTicket.Description);
        Assert.AreEqual(command.CreatorKey, addedTicket.ReporterKey);
        CollectionAssert.AreEquivalent(command.Labels.ToList(), addedTicket.Labels.ToList());
    }

    [TestMethod]
    public async Task FirstTicketHasIndexOne()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var addedTicket = updatedProject.Tickets.First();

        Assert.AreEqual(1, addedTicket.Index);
    }

    [TestMethod]
    public async Task SubsequentTicketsHaveIncrementedIndex()
    {
        var user = RegisteredUser.Default;
        var existingTicket1 = Ticket.Default(1) with
        {
            ReporterKey = user.Key,
            Title = "Ticket 1",
            Status = TicketStatus.InReview
        };
        var existingTicket2 = Ticket.Default(2) with
        {
            ReporterKey = user.Key,
            Title = "Ticket 2",
            Status = TicketStatus.InReview
        };
        var project = Project.Default with
        {
            AllowedUserKeys = [user.Key],
            Tickets = [existingTicket1, existingTicket2]
        };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        var command = new CreateTicketCommand(project.Key, "New Ticket", "Description", [], user.Key);

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var addedTicket = updatedProject.Tickets.OrderBy(t => t.Index).Last();

        Assert.AreEqual(3, addedTicket.Index);
    }

    [TestMethod]
    public async Task TicketGetsProjectDefaultStatus()
    {
        var user = RegisteredUser.Default;
        var project = Project.Default with
        {
            AllowedUserKeys = [user.Key],
            DefaultTicketStatus = TicketStatus.Open
        };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        var command = new CreateTicketCommand(project.Key, "Test Ticket", "Description", [], user.Key);

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var addedTicket = updatedProject.Tickets.First();

        Assert.AreEqual(TicketStatus.Open, addedTicket.Status);
    }

    [TestMethod]
    public async Task TicketCreatedDateIsSet()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var updatedProject = testProjectRepository.SavedEntries.First();
        var addedTicket = updatedProject.Tickets.First();

        Assert.AreNotEqual(DateTime.MinValue, addedTicket.CreatedDate);
    }

    [TestMethod]
    public async Task AdminCanCreateTicketInAnyProject()
    {
        var admin = RegisteredUser.Default with { Role = Role.Admin };
        var regularUser = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [regularUser.Key] };

        testUserRepository.AddEntryForTest(admin);
        testUserRepository.AddEntryForTest(regularUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new CreateTicketCommand(
            project.Key,
            "Admin Ticket",
            "Description",
            [],
            admin.Key);

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task AuthorizedUserCanCreateTicket()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task UnauthorizedUserCannotCreateTicket()
    {
        var authorizedUser = RegisteredUser.Default;
        var unauthorizedUser = RegisteredUser.Default;
        var project = Project.Default with { AllowedUserKeys = [authorizedUser.Key] };

        testUserRepository.AddEntryForTest(authorizedUser);
        testUserRepository.AddEntryForTest(unauthorizedUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new CreateTicketCommand(
            project.Key,
            "Unauthorized Ticket",
            "Description",
            [],
            unauthorizedUser.Key);

        await Assert.ThrowsExceptionAsync<AccessDeniedException>(
            async () => await underTest.ExecuteAsync(command));

        Assert.AreEqual(0, testProjectRepository.SavedEntries.Count);
    }
}
