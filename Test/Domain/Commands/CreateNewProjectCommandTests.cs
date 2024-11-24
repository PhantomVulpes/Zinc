using Vulpes.Electrum.Core.Domain.Exceptions;
using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;

[TestClass]
public class CreateNewProjectCommandTests
{
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly CreateNewProjectCommandHandler underTest;

    public CreateNewProjectCommandTests()
    {
        testProjectRepository = new();

        underTest = new(testProjectRepository);
    }

    private static CreateNewProjectCommand Command() => new("Test Project", "TEST", "Test Project Description", [], Guid.NewGuid());

    [TestMethod]
    public async Task ProjectIsInserted()
    {
        await underTest.ExecuteAsync(Command());

        Assert.IsTrue(testProjectRepository.Inserted.Any(), "Project was not inserted into the repository.");
    }

    [TestMethod, ExpectedException(typeof(ElectrumValidationException))]
    public async Task ProjectIsValidated()
    {
        // Projects require a four-digit shorthand.
        var command = Command() with { ProjectShorthand = "TESTING" };
        await underTest.ExecuteAsync(command);

        Assert.Fail("Project was not validated.");
    }
}