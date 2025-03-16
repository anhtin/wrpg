using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;

namespace Features.Character;

public class CreateCharacterForPlayerTest
{
    [Fact]
    public void Succeeds()
    {
        var command = CreateCommand();
        var result = CreateCharacterForPlayer.ExecuteLogic(command);

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