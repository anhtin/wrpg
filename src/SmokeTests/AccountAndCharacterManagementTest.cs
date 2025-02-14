using System.Net;
using System.Net.Http.Json;
using Microsoft.Net.Http.Headers;
using Wrpg.SmokeTests.Helpers;

namespace Wrpg.SmokeTests;

public class AccountAndCharacterManagementTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Can_create_and_view_and_delete_account_and_characters()
    {
        var identityProvider = "IP 1";
        var identityId = "ID 1";
        var accountNickname = "dylan";
        var characterName1 = "strong-dylan";
        var characterName2 = "speedy-dylan";

        await CreateAccount(identityProvider, identityId, accountNickname);
        await AssertAccountExists(accountNickname);
        await CreateCharacter(characterName1, accountNickname);
        await AssertCharacterExists(characterName1);
        await CreateCharacter(characterName2, accountNickname);
        await AssertCharacterExists(characterName2);
        await DeleteAccount(accountNickname);
        await AssertAccountNotExists(accountNickname);
        await AssertCharacterNotExists(characterName1);
        await AssertCharacterNotExists(characterName2);
    }

    private async Task CreateAccount(string identityProvider, string identityId, string nickname)
    {
        var response = await HttpClient.PostAsJsonAsync(
            "account",
            new CreateAccount.Request
            {
                IdentityProvider = identityProvider,
                IdentityId = identityId,
                Nickname = nickname,
            });

        await HttpAssert.Status(HttpStatusCode.Created, response);
        var expectedLocation = $"{HttpClient.BaseAddress}account/{nickname}";
        await HttpAssert.Header(HeaderNames.Location, expectedLocation, response);
    }

    private async Task AssertAccountExists(string nickname)
    {
        var response = await HttpClient.GetAsync($"account/{nickname}");

        var jsonOptions = Sut.GetJsonSerializerOptions();
        await HttpAssert.Status(HttpStatusCode.OK, response);
        await HttpAssert.Body(
            new GetAccount.Response { Nickname = nickname },
            response,
            jsonOptions);
    }

    private async Task AssertAccountNotExists(string nickname)
    {
        var response = await HttpClient.GetAsync($"account/{nickname}");
        await HttpAssert.Status(HttpStatusCode.NotFound, response);
    }

    private async Task DeleteAccount(string nickname)
    {
        var response = await HttpClient.DeleteAsync($"account/{nickname}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task CreateCharacter(string characterName, string accountNickname)
    {
        await HttpClient.PostAsJsonAsync("account", new CreateAccount.Request
        {
            IdentityProvider = "IP",
            IdentityId = "ID",
            Nickname = accountNickname
        });
        var response = await HttpClient.PostAsJsonAsync(
            "character",
            new CreateCharacter.Request
            {
                CharacterName = characterName,
                AccountNickname = accountNickname,
            });

        await HttpAssert.Status(HttpStatusCode.Created, response);
        var expectedLocation = $"{HttpClient.BaseAddress}character/{characterName}";
        await HttpAssert.Header(HeaderNames.Location, expectedLocation, response);
    }

    private async Task AssertCharacterExists(string name)
    {
        var response = await HttpClient.GetAsync($"character/{name}");
        await HttpAssert.Status(HttpStatusCode.OK, response);
    }

    private async Task AssertCharacterNotExists(string name)
    {
        var response = await HttpClient.GetAsync($"character/{name}");
        await HttpAssert.Status(HttpStatusCode.NotFound, response);
    }
}