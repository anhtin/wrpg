using System.Net;
using System.Net.Http.Json;
using Helpers;
using Wrpg;

public class AuthenticationTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Accepts_authenticated_request()
    {
        var response = await CreateAccount(FullyAuthorizedClient);

        await HttpAssert.Status(
            statusCode => statusCode != HttpStatusCode.Unauthorized &&
                          statusCode != HttpStatusCode.Forbidden,
            response);
    }

    [Fact]
    public async Task Rejects_unauthenticated_request()
    {
        var response = await CreateAccount(UnauthorizedClient);

        await HttpAssert.Status(HttpStatusCode.Unauthorized, response);
    }

    private static async Task<HttpResponseMessage> CreateAccount(HttpClient client)
    {
        return await client.PostAsJsonAsync(
            "account",
            new CreateAccount.Request
            {
                IdentityProvider = string.Empty,
                IdentityId = string.Empty,
                Nickname = string.Empty,
            });
    }
}