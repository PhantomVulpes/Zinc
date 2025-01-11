using Vulpes.Zinc.Domain.Commands;
using Vulpes.Zinc.Domain.Models;
using Vulpes.Zinc.Test.Doubles;
using Vulpes.Zinc.Test.Doubles.Domain.Data;

namespace Vulpes.Zinc.Test.Domain.Commands;
[TestClass]
public class UpdateProjectLabelsCommandTests
{
    private readonly TestDataRepository<Project> testProjectRepository;

    private readonly UpdateProjectLabelsCommandHandler underTest;

    public UpdateProjectLabelsCommandTests()
    {
        (_, testProjectRepository) = TestHelper.CreateTestDataRepositories();

        underTest = new(testProjectRepository);
    }

    [TestMethod]
    public async Task UpdateProjectLabelsCommandHandler_UpdatesLabels()
    {
        var project = testProjectRepository.Models.First().Value;
        var expectedLabel = "New Label";
        var command = new UpdateProjectLabelsCommand(project.Key, [expectedLabel], project.CreatorKey);

        await underTest.ExecuteAsync(command);

        var updatedProject = await testProjectRepository.GetAsync(project.Key);

        Assert.IsTrue(updatedProject.Labels.Contains(expectedLabel));
    }
}
