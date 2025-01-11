using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Doubles;
public static class TestHelper
{
    /// <summary>
    /// Builds the usual repositories and fills them with some test data.
    /// </summary>
    public static (TestDataRepository<Ticket> testTicketRepository, TestDataRepository<Project> testProjectRepository) CreateTestDataRepositories() => CreateTestDataRepositories(TicketStatus.InReview);

    /// <summary>
    /// Builds the usual repositories and fills them with some test data.
    /// </summary>
    public static (TestDataRepository<Ticket> testTicketRepository, TestDataRepository<Project> testProjectRepository) CreateTestDataRepositories(TicketStatus ticketStatus)
    {
        var zincUser = ZincUser.Default with
        {
            Username = "Test User",
        };

        var testTicketRepository = new TestDataRepository<Ticket>() { PreparedUser = zincUser };
        var testProjectRepository = new TestDataRepository<Project>() { PreparedUser = zincUser };

        var project = Project.Default with
        {
            Name = "Test Project",
            Description = "Test Project Description",
            CreatorKey = zincUser.Key,
        };

        var ticket = Ticket.Default with
        {
            Title = "Test Ticket",
            Description = "Test Ticket Description",
            ProjectKey = project.Key,
            ReporterKey = zincUser.Key,
            Status = ticketStatus
        };

        testProjectRepository.Models.Add(project.Key, project);
        testTicketRepository.Models.Add(ticket.Key, ticket);

        return (testTicketRepository, testProjectRepository);
    }
}
