using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class DeleteTicketFromDatabaseCommandTests
{
    private readonly TestDataRepository<Ticket> ticketRepository;
    private readonly TestDataRepository<Project> projectRepository;

    private readonly DeleteTicketFromDatabaseCommandHandler underTest;

    public DeleteTicketFromDatabaseCommandTests()
    {
        (ticketRepository, projectRepository) = TestHelper.CreateTestDataRepositories(TicketStatus.Cancelled);

        underTest = new DeleteTicketFromDatabaseCommandHandler(ticketRepository, projectRepository);
    }

    public static DeleteTicketFromDatabaseCommand Command(Guid ticketKey, Guid userKey) => new(ticketKey, userKey);

    [TestMethod]
    public async Task TicketIsDeleted()
    {
        await underTest.ExecuteAsync(Command(ticketRepository.Models.First().Key, ticketRepository.PreparedUser.Key));

        Assert.IsTrue(ticketRepository.Deleted.Any(), "Ticket was not deleted.");
    }

    [TestMethod]
    public async Task NonRequestedTicketWasNotDeleted()
    {
        var ticket = ticketRepository.Models.First().Value with { Key = Guid.NewGuid() };
        ticketRepository.Models.Add(ticket.Key, ticket);

        await underTest.ExecuteAsync(Command(ticketRepository.Models.First().Key, ticketRepository.PreparedUser.Key));

        Assert.IsFalse(ticketRepository.Deleted.Any(deleted => deleted.Key == ticket.Key), "Ticket was deleted.");
    }

    [TestMethod]
    public async Task TicketCannotBeDeletedWithSpecifiedStatus()
    {
        foreach (var status in Enum.GetValues<TicketStatus>())
        {
            var updatedTicket = ticketRepository.Models.First().Value with { Status = status };
            ticketRepository.Models[updatedTicket.Key] = updatedTicket;

            var result = await underTest.ValidateAccessAsync(Command(updatedTicket.Key, ticketRepository.PreparedUser.Key));

            if (status == TicketStatus.Cancelled)
            {
                Assert.IsTrue(result, $"Ticket with status {status} was not allowed to be deleted.");
            }
            else
            {
                Assert.IsFalse(result, $"Ticket with status {status} was allowed to be deleted.");
            }
        }
    }

    [TestMethod]
    public async Task InvalidUserCannotDeleteTicket()
    {
        var result = await underTest.ValidateAccessAsync(Command(ticketRepository.Models.First().Key, Guid.NewGuid()));

        Assert.IsFalse(result, "Invalid user was allowed to delete ticket.");
    }
}
