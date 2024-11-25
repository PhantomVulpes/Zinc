using Vulpes.Electrum.Core.Domain.Exceptions;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class CreateTicketCommandTests
{
    private readonly TestDataRepository<Ticket> testTicketRepository;
    private readonly TestQueryProvider<Project> testProjectQueryProvider;

    public CreateTicketCommandHandler underTest;

    public CreateTicketCommandTests()
    {
        testTicketRepository = new();
        testProjectQueryProvider = new();

        testTicketRepository.PreparedUser = ZincUser.Default;
        var testProject = Project.Default with { CreatorKey = testTicketRepository.PreparedUser.Key };

        testProjectQueryProvider.Response.Add(testProject);

        underTest = new(testTicketRepository, testProjectQueryProvider);
    }

    public static CreateTicketCommand Command(Guid projectKey, Guid userKey) => new(projectKey, "Test Ticket", "Test Ticket Description", userKey);

    [TestMethod]
    public async Task TicketIsInserted()
    {
        await underTest.ExecuteAsync(Command(testProjectQueryProvider.Response.First().Key, testTicketRepository.PreparedUser.Key));

        Assert.IsTrue(testTicketRepository.Inserted.Any(), "Ticket was not inserted into the repository.");
    }

    [TestMethod, ExpectedException(typeof(ElectrumValidationException))]
    public async Task TicketIsValidated()
    {
        var command = Command(testProjectQueryProvider.Response.First().Key, testTicketRepository.PreparedUser.Key) with { Title = string.Empty };

        await underTest.ExecuteAsync(command);

        Assert.Fail("Ticket was not validated.");
    }

    [TestMethod]
    public async Task AccessIsNotAllowed()
    {
        var result = await underTest.ValidateAccessAsync(Command(testProjectQueryProvider.Response.First().Key, Guid.NewGuid()));

        Assert.IsFalse(result, "Access was allowed.");
    }

    [TestMethod]
    public async Task AccessIsAllowed()
    {
        var result = await underTest.ValidateAccessAsync(Command(testProjectQueryProvider.Response.First().Key, testTicketRepository.PreparedUser.Key));

        Assert.IsTrue(result, "Access was not allowed.");
    }
}
