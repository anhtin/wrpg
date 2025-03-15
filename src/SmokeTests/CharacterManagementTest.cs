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
        var characterName1 = "strong-dylan";
        var characterName2 = "speedy-dylan";
        var characterName3 = "lazy-dylan";

        await CreateCharacter(characterName1);
        await AssertCharacterExists(characterName1);
        await CreateCharacter(characterName2);
        await AssertCharacterExists(characterName2);
        await CreateCharacter(characterName3);
        await AssertCharacterExists(characterName3);
        await DeleteCharacter(characterName2);
        await AssertCharacterNotExists(characterName2);
        await AssertCharacterExists(characterName1);
        await AssertCharacterExists(characterName3);
    }

    private async Task CreateCharacter(string characterName)
    {
        var response = await FullyAuthorizedClient.PostAsJsonAsync(
            "character",
            new CreateCharacter.Request { CharacterName = characterName });

        await HttpAssert.Status(HttpStatusCode.Created, response);
        var expectedLocation = $"{FullyAuthorizedClient.BaseAddress}character/{characterName}";
        await HttpAssert.Header(HeaderNames.Location, expectedLocation, response);
    }

    private async Task DeleteCharacter(string characterName)
    {
        var response = await FullyAuthorizedClient.DeleteAsync($"character/{characterName}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task AssertCharacterExists(string name)
    {
        var response = await FullyAuthorizedClient.GetAsync($"character/{name}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task AssertCharacterNotExists(string name)
    {
        var response = await FullyAuthorizedClient.GetAsync($"character/{name}");
        await HttpAssert.Status(HttpStatusCode.NotFound, response);
    }
}