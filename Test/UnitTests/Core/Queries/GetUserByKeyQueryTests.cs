using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;

namespace Vulpes.Zinc.Test.UnitTests.Core.Queries;

[TestClass]
public class GetUserByKeyQueryTests
{
    private readonly TestModelRepository<RegisteredUser> testUserRepository;

    private readonly GetUserByKeyQueryHandler underTest;

    public GetUserByKeyQueryTests()
    {
        testUserRepository = new();

        var user = RegisteredUser.Default;
        testUserRepository.Entries.Add(user.Key, user);

        underTest = new(testUserRepository);
    }

    [TestMethod]
    public async Task CanGetUserByKey()
    {
        var user = testUserRepository.Entries.First().Value;
        var query = new GetUserByKeyQuery(user.Key);

        var result = await underTest.RequestAsync(query);

        Assert.AreEqual(user.Key, result.Key);
        Assert.AreEqual(user.Username, result.Username);
    }
}
