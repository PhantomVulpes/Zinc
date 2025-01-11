using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class UpdateTicketLabelsCommandTests
{
    private readonly TestDataRepository<Ticket> testTicketRepository;
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly UpdateTicketLabelsCommandHandler underTest;

    public UpdateTicketLabelsCommandTests()
    {
        (testTicketRepository, testProjectRepository) = TestHelper.CreateTestDataRepositories();

        underTest = new UpdateTicketLabelsCommandHandler(testTicketRepository, testProjectRepository);
    }

    [TestMethod]
    public async Task ExecuteAsync_UpdatesTicketLabels()
    {
        var ticketKey = testTicketRepository.Models.First().Key;
        var expectedLabels = new[] { "Label1", "Label2" };

        var command = new UpdateTicketLabelsCommand(expectedLabels, ticketKey, testProjectRepository.Models.First().Value.CreatorKey);
        await underTest.ExecuteAsync(command);

        var updatedTicket = await testTicketRepository.GetAsync(ticketKey);
        Assert.IsTrue(expectedLabels.SequenceEqual(updatedTicket.Labels));
    }
}
