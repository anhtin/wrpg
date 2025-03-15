using System.Net;
using System.Net.Http.Json;
using Helpers;
using Wrpg;

public class AuthenticationTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Accepts_authenticated_request()
    {
        var response = await CreateCharacter(FullyAuthorizedClient);

        await HttpAssert.Status(
            statusCode => statusCode != HttpStatusCode.Unauthorized &&
                          statusCode != HttpStatusCode.Forbidden,
            response);
    }

    [Fact]
    public async Task Rejects_unauthenticated_request()
    {
        var response = await CreateCharacter(UnauthorizedClient);

        await HttpAssert.Status(HttpStatusCode.Unauthorized, response);
    }

    private static async Task<HttpResponseMessage> CreateCharacter(HttpClient client)
    {
        return await client.PostAsJsonAsync(
            "character",
            new CreateCharacter.Request { CharacterName = "dylan" });
    }
}