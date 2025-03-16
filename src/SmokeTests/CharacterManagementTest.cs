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
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        // Act & Assert
        await CustomAssert.Test("Can create characters", async () =>
        {
            await CreateCharacter(id1);
            await CreateCharacter(id2);
            await CreateCharacter(id3);
        });

        await CustomAssert.Test("Can view characters", async () =>
        {
            await AssertCharacterExists(id1);
            await AssertCharacterExists(id2);
            await AssertCharacterExists(id3);
        });

        await CustomAssert.Test("Can delete character", async () =>
        {
            await DeleteCharacter(id2);
            await AssertCharacterNotExists(id2);
            await AssertCharacterExists(id1);
            await AssertCharacterExists(id3);
        });
    }

    private async Task CreateCharacter(Guid characterId)
    {
        const string characterName = "Dylan";
        var request = JsonContent.Create(new CreateCharacterForPlayer.Request { CharacterName = characterName });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, characterId.ToString());
        var response = await PlayerClient.PostAsync("character", request);

        await HttpAssert.Status(HttpStatusCode.Created, response);
        var expectedLocation = $"{PlayerClient.BaseAddress}character/{characterId}";
        await HttpAssert.Header(HeaderNames.Location, expectedLocation, response);
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