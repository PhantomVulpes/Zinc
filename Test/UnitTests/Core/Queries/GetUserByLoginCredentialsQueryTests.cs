using System.Security.Authentication;
using Vulpes.Zinc.Core.Models;
using Vulpes.Zinc.Core.Queries;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Data;
using Vulpes.Zinc.Test.Doubles.Electrum.Domain.Security;

namespace Vulpes.Zinc.Test.UnitTests.Core.Queries;

[TestClass]
public class GetUserByLoginCredentialsQueryTests
{
    private readonly TestQueryProvider<RegisteredUser> testUserQueryProvider;
    private readonly TestKnoxHasher testKnoxHasher;

    private readonly GetUserByLoginCredentialsQueryHandler underTest;

    private const string TestPassword = "test-password";
    private const string TestUsername = "TestUser";

    public GetUserByLoginCredentialsQueryTests()
    {
        testUserQueryProvider = new();
        testKnoxHasher = new();

        var user = RegisteredUser.Default with
        {
            Username = TestUsername,
            PasswordHash = testKnoxHasher.MockedHashResult
        };

        testUserQueryProvider.Response = [user];

        underTest = new(testUserQueryProvider, testKnoxHasher);
    }

    [TestMethod]
    public async Task ValidCredentials_ReturnsUser()
    {
        testKnoxHasher.HashResults.Add((TestPassword, true));
        var query = new GetUserByLoginCredentialsQuery(TestUsername, TestPassword);

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(TestUsername, result.Username);
    }

    [TestMethod]
    public async Task InvalidUsername_ThrowsInvalidCredentialException()
    {
        testKnoxHasher.HashResults.Add((TestPassword, true));
        var query = new GetUserByLoginCredentialsQuery("WrongUsername", TestPassword);

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task InvalidPassword_ThrowsInvalidCredentialException()
    {
        testKnoxHasher.HashResults.Add((TestPassword, false));
        var query = new GetUserByLoginCredentialsQuery(TestUsername, TestPassword);

        await Assert.ThrowsExceptionAsync<InvalidCredentialException>(
            async () => await underTest.RequestAsync(query)
        );
    }

    [TestMethod]
    public async Task UsernameMatching_IsCaseInsensitive()
    {
        testKnoxHasher.HashResults.Add((TestPassword, true));
        var query = new GetUserByLoginCredentialsQuery("testuser", TestPassword);

        var result = await underTest.RequestAsync(query);

        Assert.IsNotNull(result);
        Assert.AreEqual(TestUsername, result.Username);
    }
}
