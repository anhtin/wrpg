using System.Net;
using System.Net.Http.Json;
using Helpers;
using Microsoft.Net.Http.Headers;
using Wrpg;

public class CharacterManagementTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Can_create_and_view_and_delete_characters()
    {
        var id1 = await CreateCharacter("strong-dylan");
        await AssertCharacterExists(id1);
        var id2 = await CreateCharacter("speedy-dylan");
        await AssertCharacterExists(id2);
        var id3 = await CreateCharacter("lazy-dylan");
        await AssertCharacterExists(id3);
        await DeleteCharacter(id2);
        await AssertCharacterNotExists(id2);
        await AssertCharacterExists(id1);
        await AssertCharacterExists(id3);
    }

    private async Task<Guid> CreateCharacter(string characterName)
    {
        var characterId = Guid.NewGuid();
        var request = JsonContent.Create(new CreateCharacterForPlayer.Request { CharacterName = characterName });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, characterId.ToString());
        var response = await PlayerClient.PostAsync("character", request);

        await HttpAssert.Status(HttpStatusCode.Created, response);
        var expectedLocation = $"{PlayerClient.BaseAddress}character/{characterId}";
        await HttpAssert.Header(HeaderNames.Location, expectedLocation, response);

        return characterId;
    }

    private async Task DeleteCharacter(Guid id)
    {
        var response = await PlayerClient.DeleteAsync($"character/{id}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task AssertCharacterExists(Guid id)
    {
        var response = await PlayerClient.GetAsync($"character/{id}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task AssertCharacterNotExists(Guid id)
    {
        var response = await PlayerClient.GetAsync($"character/{id}");
        await HttpAssert.Status(HttpStatusCode.NotFound, response);
    }
}