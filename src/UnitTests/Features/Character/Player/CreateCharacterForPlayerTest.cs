using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg;

namespace Features.Character.Player;

public class CreateCharacterForPlayerTest
{
    [Theory]
    [InlineData(" Leading space", "Leading space")]
    [InlineData("Trailing space ", "Trailing space")]
    [InlineData(" Space on both ends ", "Space on both ends")]
    public void Success_trims_character_name(string characterName, string trimmedCharacterName)
    {
        var command = CreateCommand(characterName: characterName);
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        Assert.IsType<CreatedAtRoute>(result.Http);
        Assert.NotNull(result.SideEffects);
        Assert.Equal(trimmedCharacterName, result.SideEffects.CreateCharacter.Entity.Name);
    }

    [Theory]
    [InlineData(CharacterName.MinLength)]
    [InlineData(((CharacterName.MaxLength - CharacterName.MinLength) / 2) + CharacterName.MinLength)]
    [InlineData(CharacterName.MaxLength)]
    public void Succeeds_when_character_name_is_not_empty_and_does_not_exceed_max_length(int length)
    {
        var command = CreateCommand(characterName: Generator.RandomString(length, includeSpace: false));
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        AssertSuccess(result, command);
    }

    private static void AssertSuccess(
        FeatureResult<CreateCharacterForPlayer.SideEffects?> result,
        CreateCharacterForPlayer.Command command)
    {
        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<CreatedAtRoute>(result.Http);
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
    [InlineData(CharacterName.MaxLength + 1)]
    [InlineData(CharacterName.MaxLength + 2)]
    public void Fails_when_character_name_exceeds_max_length(int length)
    {
        var command = CreateCommand(characterName: Generator.RandomString(length, includeSpace: false));
        var result = CreateCharacterForPlayer.ExecuteLogic(command);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http);
            Assert.NotNull(subject.Value);
            Assert.Equal(CharacterName.MaxLengthErrorMessage, subject.Value.Detail);
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
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http);
            Assert.NotNull(subject.Value);
            Assert.Equal(CharacterName.MinLengthErrorMessage, subject.Value.Detail);
        });
    }

    private static void AssertFailure(
        FeatureResult<CreateCharacterForPlayer.SideEffects?> result,
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
        CharacterName = characterName ?? Generator.RandomString(CharacterName.MaxLength),
        UserId = userId ?? Generator.RandomString(UserId.MaxLength),
    };
}