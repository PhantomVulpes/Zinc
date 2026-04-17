using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Zinc.Core.Commands;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Security;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Commands;

[TestClass]
public class AddCommentToTicketCommandTests
{
    private readonly AddCommentToTicketCommandHandler underTest;

    private readonly TestModelRepository<RegisteredUser> testUserRepository;
    private readonly TestModelRepository<Project> testProjectRepository;

    public AddCommentToTicketCommandTests()
    {
        testUserRepository = new();
        testProjectRepository = new();

        underTest = new(testUserRepository, testProjectRepository);
    }

    private AddCommentToTicketCommand PrepareCommand()
    {
        var user = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with
        {
            ReporterKey = user.Key,
            Title = "Test Ticket",
            Status = TicketStatus.InReview
        };
        var project = Project.Default with { AllowedUserKeys = [user.Key], Tickets = [ticket] };
        testUserRepository.AddEntryForTest(user);
        testProjectRepository.AddEntryForTest(project);

        return new(project.Key, ticket.Index, "This is a test comment", user.Key);
    }

    [TestMethod]
    public async Task ProjectSavesToRepository()
    {
        await underTest.ExecuteAsync(PrepareCommand());

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task CommentIsAddedToTicket()
    {
        var command = PrepareCommand();

        var initialTicket = testProjectRepository.Entries.First().Value.Tickets.First();
        var initialCommentCount = initialTicket.Comments.Count();

        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();
        var updatedCommentCount = updatedTicket.Comments.Count();

        Assert.AreEqual(initialCommentCount + 1, updatedCommentCount);
    }

    [TestMethod]
    public async Task CommentPropertiesAreSetCorrectly()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        var updatedTicket = testProjectRepository.SavedEntries.First().Tickets.First();
        var addedComment = updatedTicket.Comments.Last();

        Assert.AreEqual(command.Value, addedComment.Value);
        Assert.AreEqual(command.UserKey, addedComment.Author);
        Assert.AreNotEqual(DateTime.MinValue, addedComment.CreatedDate);
    }

    [TestMethod]
    public async Task AdminCanAddCommentToAnyTicket()
    {
        var admin = RegisteredUser.Default with { Role = Role.Admin };
        var regularUser = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with
        {
            ReporterKey = regularUser.Key,
            Title = "Test Ticket",
            Status = TicketStatus.InReview
        };
        var project = Project.Default with { AllowedUserKeys = [regularUser.Key], Tickets = [ticket] };

        testUserRepository.AddEntryForTest(admin);
        testUserRepository.AddEntryForTest(regularUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new AddCommentToTicketCommand(
            project.Key,
            ticket.Index,
            "Admin comment",
            admin.Key);

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task AuthorizedUserCanAddComment()
    {
        var command = PrepareCommand();

        await underTest.ExecuteAsync(command);

        Assert.AreEqual(1, testProjectRepository.SavedEntries.Count);
    }

    [TestMethod]
    public async Task UnauthorizedUserCannotAddComment()
    {
        var authorizedUser = RegisteredUser.Default;
        var unauthorizedUser = RegisteredUser.Default;
        var ticket = Ticket.Default(1) with
        {
            ReporterKey = authorizedUser.Key,
            Title = "Test Ticket",
            Status = TicketStatus.InReview
        };
        var project = Project.Default with { AllowedUserKeys = [authorizedUser.Key], Tickets = [ticket] };

        testUserRepository.AddEntryForTest(authorizedUser);
        testUserRepository.AddEntryForTest(unauthorizedUser);
        testProjectRepository.AddEntryForTest(project);

        var command = new AddCommentToTicketCommand(
            project.Key,
            ticket.Index,
            "Unauthorized comment",
            unauthorizedUser.Key);

        await Assert.ThrowsExceptionAsync<AccessDeniedException>(
            async () => await underTest.ExecuteAsync(command));

        Assert.AreEqual(0, testProjectRepository.SavedEntries.Count);
    }
}
