using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class UpdateTicketDescriptionCommandTests
{
    private readonly TestDataRepository<Ticket> testTicketRepository;
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly UpdateTicketDescriptionCommandHandler underTest;

    public UpdateTicketDescriptionCommandTests()
    {
        (testTicketRepository, testProjectRepository) = TestHelper.CreateTestDataRepositories();

        underTest = new UpdateTicketDescriptionCommandHandler(testTicketRepository, testProjectRepository);
    }

    [TestMethod]
    public async Task ExecuteAsync_UpdatesTicketDescription()
    {
        var ticketKey = testTicketRepository.Models.First().Key;
        var expectedDescription = "New description";

        var command = new UpdateTicketDescriptionCommand(ticketKey, expectedDescription, testProjectRepository.Models.First().Value.CreatorKey);
        await underTest.ExecuteAsync(command);

        var updatedTicket = await testTicketRepository.GetAsync(ticketKey);
        Assert.AreEqual(expectedDescription, updatedTicket.Description);
    }
}
