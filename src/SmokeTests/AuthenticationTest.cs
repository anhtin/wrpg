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
        await HttpAssert.Status(HttpStatusCode.Created, response);
    }

    [Fact]
    public async Task Rejects_unauthenticated_request()
    {
        var response = await CreateCharacter(UnauthorizedClient);

        await HttpAssert.Status(HttpStatusCode.Unauthorized, response);
    }

    private static async Task<HttpResponseMessage> CreateCharacter(HttpClient client)
    {
        var request = JsonContent.Create(new CreateCharacter.Request { CharacterName = "dylan" });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, Guid.NewGuid().ToString());
        return await client.PostAsync("character", request);
    }
}