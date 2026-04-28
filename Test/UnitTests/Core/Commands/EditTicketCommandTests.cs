using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class EditTicketCommandTests
{
    private readonly EditTicketCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    public EditTicketCommandTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        underTest = new(testUserRepository, testProjectRepository);
    }

    private EditTicketCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with { ReporterKey = user.Key };
        var project = Project.Default with { AllowedUserKeys = [user.Key], Tickets = [ticket] };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        return new(project.Key, ticket.Index, "Test title", "Test description", [], TicketStatus.InProgress, user.Key);
    }

    [TestMethod]
    public async Task TicketSavesToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task TicketValuesAreUpdated()
    {
        var command = PrepareCommand();

        // Get the ticket then run the test.
        var initialTicket = testProjectRepository.Entries.First().Value.Tickets.First();
        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreNotEqual(initialTicket.Title, updatedTicket.Title);
        Assert.AreEqual(command.Title, updatedTicket.Title);
    }

    [TestMethod]
    public async Task CompletedTicketGetsTimeStamped()
    {
        var command = PrepareCommand() with { Status = TicketStatus.Complete };

        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        // Building a new default ticket just in case the default changes. It won't, but just in case.
        Assert.AreNotEqual(Ticket.Default(1).CompletedDate, updatedTicket.CompletedDate);
    }

    [TestMethod]
    public async Task DescriptionIsUpdated()
    {
        var command = PrepareCommand();

        var initialTicket = testProjectRepository.Entries.First().Value.Tickets.First();
        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreNotEqual(initialTicket.Description, updatedTicket.Description);
        Assert.AreEqual(command.Description, updatedTicket.Description);
    }

    [TestMethod]
    public async Task LabelsAreUpdated()
    {
        var command = PrepareCommand() with { Labels = new[] { "bug", "urgent" } };

        var initialTicket = testProjectRepository.Entries.First().Value.Tickets.First();
        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreNotEqual(initialTicket.Labels.Count(), updatedTicket.Labels.Count());
        CollectionAssert.AreEquivalent(command.Labels.ToList(), updatedTicket.Labels.ToList());
    }

    [TestMethod]
    public async Task StatusIsUpdated()
    {
        var command = PrepareCommand() with { Status = TicketStatus.InReview };

        var initialTicket = testProjectRepository.Entries.First().Value.Tickets.First();
        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreNotEqual(initialTicket.Status, updatedTicket.Status);
        Assert.AreEqual(TicketStatus.InReview, updatedTicket.Status);
    }

    [TestMethod]
    public async Task NonCompleteStatusDoesNotSetCompletedDate()
    {
        var command = PrepareCommand() with { Status = TicketStatus.InReview };

        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreEqual(DateTime.MinValue, updatedTicket.CompletedDate);
    }

    [TestMethod]
    public async Task AlreadyCompleteTicketDoesNotUpdateCompletedDate()
    {
        var completedDate = DateTime.UtcNow.AddDays(-5);
        var user = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with
        {
            ReporterKey = user.Key,
            Status = TicketStatus.Complete,
            CompletedDate = completedDate
        };
        var project = Project.Default with { AllowedUserKeys = [user.Key], Tickets = [ticket] };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditTicketCommand(
            project.Key,
            ticket.Index,
            "Updated title",
            "Updated description",
            [],
            TicketStatus.Complete,
            user.Key);

        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();

        Assert.AreEqual(completedDate, updatedTicket.CompletedDate);
    }

    [TestMethod]
    public async Task AdminCanEditAnyTicket()
    {
        var admin = RegisteredUser.Default with { Role = Role.Admin };
        var regularUser = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with { ReporterKey = regularUser.Key };
        var project = Project.Default with { AllowedUserKeys = [regularUser.Key], Tickets = [ticket] };

        testUserRepository.AddEntryForTest(admin);
        testUserRepository.AddEntryForTest(regularUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditTicketCommand(
            project.Key,
            ticket.Index,
            "Admin edit",
            "Description",
            [],
            TicketStatus.InProgress,
            admin.Key);

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task AuthorizedUserCanEditTicket()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task UnauthorizedUserCannotEditTicket()
    {
        var authorizedUser = RegisteredUser.Default;
        var unauthorizedUser = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with { ReporterKey = authorizedUser.Key };
        var project = Project.Default with { AllowedUserKeys = [authorizedUser.Key], Tickets = [ticket] };

        testUserRepository.AddEntryForTest(authorizedUser);
        testUserRepository.AddEntryForTest(unauthorizedUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new EditTicketCommand(
            project.Key,
            ticket.Index,
            "Unauthorized edit",
            "Description",
            [],
            TicketStatus.InProgress,
            unauthorizedUser.Key);

        await Assert.ThrowsExceptionAsync<AccessDeniedException>(
            async () => await underTest.ExecuteAsync(command));

        Assert.AreEqual(0, testProjectRepository.SavedEntries.Count);
    }
}
