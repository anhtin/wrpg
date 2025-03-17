using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;

namespace Features.Adventure.Player;

public class GetAdventureForPlayerTest
{
    [Fact]
    public async Task Fails_when_adventure_does_not_exist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = ClaimsPrincipalGenerator.Create(userId: userId);
        var adventure = AdventureGenerator.Create();
        var dbContext = AppDbContextGenerator.Create(entities: [adventure]);

        // Act
        var response = await GetAdventureForPlayer.Execute(Guid.NewGuid(), user, dbContext);

        // Assert
        Assert.IsType<NotFound>(response.Result);
    }

    [Theory]
    [InlineData(AdventureStatus.Ongoing)]
    [InlineData(AdventureStatus.Ended)]
    public async Task Success_returns_adventure_with_correct_information(AdventureStatus status)
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var user = ClaimsPrincipalGenerator.Create(userId: userId);
        var adventure = AdventureGenerator.Create(
            id: Guid.NewGuid(),
            userId: userId,
            characterId: Guid.NewGuid(),
            name: Generator.RandomString(),
            status: status,
            locationName: Generator.RandomString(AdventureName.MaxLength));
        var dbContext = AppDbContextGenerator.Create(entities: [adventure]);

        // Act
        var response = await GetAdventureForPlayer.Execute(adventure.Id, user, dbContext);

        // Assert
        var result = Assert.IsType<Ok<GetAdventureForPlayer.Adventure>>(response.Result);
        var expected = Map(adventure);
        Assert.Equivalent(expected, result.Value);
    }

    private static GetAdventureForPlayer.Adventure Map(Wrpg.Adventure adventure) => new()
    {
        CharacterId = adventure.CharacterId,
        Name = adventure.Name,
        Status = adventure.Status,
        LocationName = adventure.LocationName,
    };
}