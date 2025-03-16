using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;

namespace Features.Character.Player;

public class GetCharacterForPlayerTest
{
    [Fact]
    public async Task Fails_when_character_does_not_exist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = ClaimsPrincipalGenerator.Create(userId: userId);
        var character = CharacterGenerator.Create();
        var dbContext = AppDbContextGenerator.Create(entities: [character]);

        // Act
        var response = await GetCharacterForPlayer.Execute(Guid.NewGuid(), user, dbContext);

        // Assert
        Assert.IsType<NotFound>(response.Result);
    }

    [Fact]
    public async Task Returns_character_with_correct_information()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = ClaimsPrincipalGenerator.Create(userId: userId);
        var character = CharacterGenerator.Create(
            userId: userId,
            id: Guid.NewGuid(),
            name: Generator.RandomString(),
            stats: CreateRandomStats());
        var dbContext = AppDbContextGenerator.Create(entities: [character]);

        // Act
        var response = await GetCharacterForPlayer.Execute(character.Id, user, dbContext);

        // Assert
        var result = Assert.IsType<Ok<GetCharacterForPlayer.Character>>(response.Result);
        var expected = Map(character);
        Assert.Equivalent(expected, result.Value);
    }

    private static Stats CreateRandomStats() => new()
    {
        Attributes = new()
        {
            Level = Generator.RandomInt(minValue: 0),
            Strength = Generator.RandomInt(minValue: 0),
            Constitution = Generator.RandomInt(minValue: 0),
            Dexterity = Generator.RandomInt(minValue: 0),
            Intelligence = Generator.RandomInt(minValue: 0),
            Spirit = Generator.RandomInt(minValue: 0),
        },
        Resources = new()
        {
            Health = Generator.RandomInt(minValue: 0),
            Energy = Generator.RandomInt(minValue: 0),
        },
    };

    private static GetCharacterForPlayer.Character Map(Wrpg.Character character) => new()
    {
        Name = character.Name,
        Stats = new GetCharacterForPlayer.Stats
        {
            Attributes = new()
            {
                Level = character.Stats.Attributes.Level,
                Strength = character.Stats.Attributes.Strength,
                Dexterity = character.Stats.Attributes.Dexterity,
                Intelligence = character.Stats.Attributes.Intelligence,
                Constitution = character.Stats.Attributes.Constitution,
                Spirit = character.Stats.Attributes.Spirit,
            },
            Resources = new()
            {
                Health = character.Stats.Resources.Health,
                Energy = character.Stats.Resources.Energy,
            },
        },
    };
}