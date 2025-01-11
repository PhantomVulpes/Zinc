using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class UpdateTicketStatusCommandTests
{
    private readonly TestDataRepository<Ticket> testTicketRepository;
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly UpdateTicketStatusCommandHandler underTest;

    public UpdateTicketStatusCommandTests()
    {
        (testTicketRepository, testProjectRepository) = TestHelper.CreateTestDataRepositories();

        underTest = new UpdateTicketStatusCommandHandler(testTicketRepository, testProjectRepository);
    }

    [TestMethod]
    public async Task ExecuteAsync_UpdatesTicketStatus()
    {
        var ticketKey = testTicketRepository.Models.First().Key;
        var expectedStatus = TicketStatus.Cancelled;

        var command = new UpdateTicketStatusCommand(ticketKey, expectedStatus, testProjectRepository.Models.First().Value.CreatorKey);
        await underTest.ExecuteAsync(command);

        var updatedTicket = await testTicketRepository.GetAsync(ticketKey);
        Assert.AreEqual(expectedStatus, updatedTicket.Status);
    }
}
