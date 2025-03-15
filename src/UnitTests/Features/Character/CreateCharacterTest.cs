using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg;
using Wrpg.Shared.SideEffects;

namespace Features.Character;

public class CreateCharacterTest
{
    [Fact]
    public void Succeeds_when_all_is_good()
    {
        var command = CreateCommand();
        var result = CreateCharacter.ExecuteLogic(command);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<CreatedAtRoute>(result.Http.Result);
                Assert.Multiple(
                    () => Assert.Equal(nameof(GetCharacter), subject.RouteName),
                    () =>
                    {
                        var expected = new KeyValuePair<string, object?>("Name", command.CharacterName);
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
                            userId: command.UserId,
                            name: command.CharacterName,
                            stats: Stats.CreateNew()));
                        Assert.Equivalent(expected, subject.CreateCharacter);
                    });
            });
    }

    [Fact]
    public void Fails_when_character_name_is_bad()
    {
        var characterName = "Invalid character name";
        Assert.False(CharacterName.IsValid(characterName));

        var command = CreateCommand(characterName: characterName);
        var result = CreateCharacter.ExecuteLogic(command);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
                var expected = CreateCharacter.BadCharacterNameMessage;
                Assert.NotNull(subject.Value);
                Assert.Equal(expected, subject.Value.Detail);
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    private static CreateCharacter.Command CreateCommand(
        string? characterName = null,
        string? userId = null) => new()
    {
        CharacterName = characterName ?? Generator.RandomString(),
        UserId = userId ?? Generator.RandomString(),
    };
}