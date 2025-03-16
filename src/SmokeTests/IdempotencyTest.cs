using System.Net;
using System.Net.Http.Json;
using Helpers;
using Wrpg;

public class IdempotencyTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task CreateCharacter_succeeds_when_subsequent_request_is_identical()
    {
        var idempotencyKey = Guid.NewGuid();
        const string name = "dylan";
        await CreateCharacter(PlayerClient, idempotencyKey, name);
        var response = await CreateCharacter(PlayerClient, idempotencyKey, name);
        await HttpAssert.Status(HttpStatusCode.Created, response);
    }

    [Fact]
    public async Task CreateCharacter_fails_when_subsequent_request_is_different_but_idempotency_key_is_identical()
    {
        var idempotencyKey = Guid.NewGuid();
        await CreateCharacter(PlayerClient, idempotencyKey, "dylan");
        var response = await CreateCharacter(PlayerClient, idempotencyKey, "bob");
        await HttpAssert.Status(HttpStatusCode.Conflict, response);
    }

    private static async Task<HttpResponseMessage> CreateCharacter(HttpClient client, Guid idempotencyKey, string name)
    {
        var request = JsonContent.Create(new CreateCharacterForPlayer.Request { CharacterName = name });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, idempotencyKey.ToString());
        return await client.PostAsync("character", request);
    }
}