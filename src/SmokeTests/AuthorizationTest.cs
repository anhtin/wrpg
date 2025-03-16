using System.Net;
using System.Net.Http.Json;
using Helpers;
using Wrpg;

public class AuthorizationTest(Sut sut) : SmokeTestContext(sut)
{
    [Fact]
    public async Task Unauthenticated_user_cannot_create_or_view_character()
    {
        await CustomAssert.Test("Cannot create character (through player endpoint)", async () =>
        {
            var response = await CreateCharacterForPlayer(UnauthorizedClient, Guid.NewGuid());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        });

        await CustomAssert.Test("Cannot view character (through player endpoint)", async () =>
        {
            var response = await ViewCharacterForPlayer(UnauthorizedClient, Guid.NewGuid());
            await HttpAssert.Status(HttpStatusCode.Unauthorized, response);
        });
    }

    [Fact]
    public async Task Player_can_create_and_view_own_character()
    {
        // Arrange
        var id = Guid.NewGuid();

        var response = await CreateCharacterForPlayer(Player1Client, id);
        await HttpAssert.Status(HttpStatusCode.Created, response);

        await CustomAssert.Test("Can view other player's character (through player endpoint)", async () =>
        {
            response = await ViewCharacterForPlayer(Player1Client, id);
            await HttpAssert.Status(HttpStatusCode.OK, response);
        });

        await CustomAssert.Test("Cannot view other player's character (through admin endpoint)", async () =>
        {
            response = await ViewCharacterForAdmin(Player1Client, id);
            await HttpAssert.Status(HttpStatusCode.Forbidden, response);
        });
    }

    [Fact]
    public async Task Player_cannot_view_other_player_characters()
    {
        // Arrange
        var id = Guid.NewGuid();

        var response = await CreateCharacterForPlayer(Player2Client, id);
        await HttpAssert.Status(HttpStatusCode.Created, response);

        response = await ViewCharacterForPlayer(Player2Client, id);
        await HttpAssert.Status(HttpStatusCode.OK, response);

        // Act & Assert
        await CustomAssert.Test("Cannot view other player's character (through player endpoint)", async () =>
        {
            response = await ViewCharacterForPlayer(Player1Client, id);
            await HttpAssert.Status(HttpStatusCode.NotFound, response);
        });

        await CustomAssert.Test("Cannot view other player's character (through admin endpoint)", async () =>
        {
            response = await ViewCharacterForAdmin(Player1Client, id);
            await HttpAssert.Status(HttpStatusCode.Forbidden, response);
        });
    }

    [Fact]
    public async Task Admin_can_view_and_delete_any_player_character()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var response = await CreateCharacterForPlayer(Player1Client, id1);
        await HttpAssert.Status(HttpStatusCode.Created, response);

        response = await CreateCharacterForPlayer(Player2Client, id2);
        await HttpAssert.Status(HttpStatusCode.Created, response);

        // Act & Assert
        await CustomAssert.Test("Can view any player character (through admin endpoint)", async () =>
        {
            response = await ViewCharacterForAdmin(AdminClient, id1);
            await HttpAssert.Status(HttpStatusCode.OK, response);

            response = await ViewCharacterForAdmin(AdminClient, id2);
            await HttpAssert.Status(HttpStatusCode.OK, response);
        });

        await CustomAssert.Test("Cannot view any player character (through player endpoint)", async () =>
        {
            response = await ViewCharacterForPlayer(AdminClient, id1);
            await HttpAssert.Status(HttpStatusCode.Forbidden, response);

            response = await ViewCharacterForPlayer(AdminClient, id2);
            await HttpAssert.Status(HttpStatusCode.Forbidden, response);
        });

        await CustomAssert.Test("Can delete any player character", async () =>
        {
            response = await DeleteCharacterForAdmin(AdminClient, id1);
            await HttpAssert.Status(HttpStatusCode.OK, response);
            response = await ViewCharacterForAdmin(AdminClient, id1);
            await HttpAssert.Status(HttpStatusCode.NotFound, response);

            response = await DeleteCharacterForAdmin(AdminClient, id2);
            await HttpAssert.Status(HttpStatusCode.OK, response);
            response = await ViewCharacterForAdmin(AdminClient, id2);
            await HttpAssert.Status(HttpStatusCode.NotFound, response);
        });
    }

    private static async Task<HttpResponseMessage> CreateCharacterForPlayer(HttpClient client, Guid characterId)
    {
        const string characterName = "Dylan";
        var request = JsonContent.Create(new CreateCharacterForPlayer.Request { CharacterName = characterName });
        request.Headers.Add(CustomHttpHeader.IdempotencyKey, characterId.ToString());
        return await client.PostAsync("character", request);
    }

    private static async Task<HttpResponseMessage> ViewCharacterForPlayer(HttpClient client, Guid id)
    {
        return await client.GetAsync($"character/{id}");
    }

    private static async Task<HttpResponseMessage> ViewCharacterForAdmin(HttpClient client, Guid id)
    {
        return await client.GetAsync($"admin/character/{id}");
    }

    private static async Task<HttpResponseMessage> DeleteCharacterForAdmin(HttpClient client, Guid id)
    {
        return await client.DeleteAsync($"admin/character/{id}");
    }
}