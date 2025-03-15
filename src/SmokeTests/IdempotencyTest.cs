using System.Net;
using System.Net.Http.Json;
using Helpers;
using Wrpg;

public class IdempotencyTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Responds_with_201_Created_when_request_is_identical()
    {
        var idempotencyKey = Guid.NewGuid();
        const string name = "dylan";
        await CreateCharacter(FullyAuthorizedClient, idempotencyKey, name);
        var response = await CreateCharacter(FullyAuthorizedClient, idempotencyKey, name);
        await HttpAssert.Status(HttpStatusCode.Created, response);
    }

    [Fact]
    public async Task Responds_with_409_Conflict_when_request_is_different_but_idempotency_key_is_identical()
    {
        var idempotencyKey = Guid.NewGuid();
        await CreateCharacter(FullyAuthorizedClient, idempotencyKey, "dylan");
        var response = await CreateCharacter(FullyAuthorizedClient, idempotencyKey, "bob");
        await HttpAssert.Status(HttpStatusCode.Conflict, response);
    }

    private static async Task<HttpResponseMessage> CreateCharacter(HttpClient client, Guid idempotencyKey, string name)
    {
        var request = JsonContent.Create(new CreateCharacter.Request { CharacterName = name });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, idempotencyKey.ToString());
        return await client.PostAsync("character", request);
    }
}