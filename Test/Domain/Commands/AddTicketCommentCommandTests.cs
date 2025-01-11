using Vulpes.Electrum.Core.Domain.Exceptions;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class AddTicketCommentCommandTests
{
    private readonly TestDataRepository<Ticket> testTicketRepository;
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly AddTicketCommentCommandHandler underTest;

    public AddTicketCommentCommandTests()
    {
        (testTicketRepository, testProjectRepository) = TestHelper.CreateTestDataRepositories();

        underTest = new(testTicketRepository, testProjectRepository);
    }

    [TestMethod]
    public async Task CommentIsAdded()
    {
        var command = new AddTicketCommentCommand("Test Comment", testTicketRepository.Models.First().Key, testProjectRepository.PreparedUser.Key);
        await underTest.ExecuteAsync(command);

        var ticket = testTicketRepository.Models.First().Value;

        Assert.IsTrue(ticket.Comments.Any(), "Comment was not added.");
    }

    [TestMethod]
    public async Task TicketIsSaved()
    {
        var command = new AddTicketCommentCommand("Test Comment", testTicketRepository.Models.First().Key, testProjectRepository.PreparedUser.Key);
        await underTest.ExecuteAsync(command);

        Assert.IsTrue(testTicketRepository.Saved.Any(), "No ticket was saved.");
    }

    [TestMethod, ExpectedException(typeof(ElectrumValidationException))]
    public async Task TicketIsValidated()
    {
        // Use an empty ticket to force an invalid result.
        var ticket = Ticket.Empty with { ProjectKey = testProjectRepository.Models.First().Key };
        testTicketRepository.Models.Add(ticket.Key, ticket);

        var command = new AddTicketCommentCommand("Test Comment", ticket.Key, testProjectRepository.PreparedUser.Key);
        await underTest.ExecuteAsync(command);

        Assert.Fail("Ticket is empty, should have thrown invalid exception.");
    }

    [TestMethod]
    public async Task UserCannotAddCommentForProjectTheyDoNotHaveAccessTo()
    {
        var newUser = ZincUser.Default with { Key = Guid.NewGuid() };
        var command = new AddTicketCommentCommand("Test Comment", testTicketRepository.Models.First().Key, newUser.Key);
        var accessResult = await underTest.ValidateAccessAsync(command);

        Assert.IsFalse(accessResult, "User was allowed to comment on a project they are not allowed on.");
    }

    [TestMethod]
    public async Task UserCanAddCommentForProjectTheyHaveAccessTo()
    {
        var command = new AddTicketCommentCommand("Test Comment", testTicketRepository.Models.First().Key, testTicketRepository.PreparedUser.Key);
        var accessResult = await underTest.ValidateAccessAsync(command);

        Assert.IsTrue(accessResult, "User was not allowed to comment on a project they have access to.");
    }
}
