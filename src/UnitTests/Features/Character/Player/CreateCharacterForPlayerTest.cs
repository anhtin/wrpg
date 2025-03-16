using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg;

namespace Features.Character.Player;

using HttpResult = Results<CreatedAtRoute, Conflict, BadRequest<ProblemDetails>>;

public class CreateCharacterForPlayerTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(20)]
    public void Succeeds_when_character_name_is_not_empty_and_does_not_exceed_max_length(int length)
    {
        var command = CreateCommand(characterName: Generator.RandomString(length));
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        AssertSuccess(result, command);
    }

    private static void AssertSuccess(
        FeatureResult<HttpResult, CreateCharacterForPlayer.SideEffects?> result,
        CreateCharacterForPlayer.Command command)
    {
        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<CreatedAtRoute>(result.Http.Result);
                Assert.Multiple(
                    () => Assert.Equal(nameof(GetCharacterForPlayer), subject.RouteName),
                    () =>
                    {
                        var expected = new KeyValuePair<string, object?>("Id", command.CharacterId);
                        Assert.Contains(expected, subject.RouteValues);
                    });
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.NotNull(subject);
                Assert.Multiple(
                    () => Assert.IsType<CreateEntity<Wrpg.Character>>(subject.CreateCharacter),
                    () =>
                    {
                        var expected = new CreateEntity<Wrpg.Character>(CharacterGenerator.Create(
                            id: command.CharacterId,
                            userId: command.UserId,
                            name: command.CharacterName,
                            stats: Stats.CreateNew()));
                        Assert.Equivalent(expected, subject.CreateCharacter);
                    });
            });
    }

    [Theory]
    [InlineData(21)]
    [InlineData(22)]
    public void Fails_when_character_name_exceeds_max_length(int length)
    {
        var command = CreateCommand(characterName: Generator.RandomString(length));
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
            Assert.NotNull(subject.Value);
            Assert.Equal(CreateCharacterForPlayer.CharacterNameExceedsMaxLengthMessage, subject.Value.Detail);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Fails_when_character_name_is_empty(string characterName)
    {
        var command = CreateCommand(characterName: characterName);
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
            Assert.NotNull(subject.Value);
            Assert.Equal(CreateCharacterForPlayer.CharacterNameIsEmptyMessage, subject.Value.Detail);
        });
    }

    private static void AssertFailure(
        FeatureResult<HttpResult, CreateCharacterForPlayer.SideEffects?> result,
        Action additionalTestCode)
    {
        Assert.Multiple(
            additionalTestCode,
            () => Assert.Null(result.SideEffects));
    }

    private static CreateCharacterForPlayer.Command CreateCommand(
        Guid? characterId = null,
        string? characterName = null,
        string? userId = null) => new()
    {
        CharacterId = characterId ?? Guid.NewGuid(),
        CharacterName = characterName ?? Generator.RandomString(),
        UserId = userId ?? Generator.RandomString(),
    };
}