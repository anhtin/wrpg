using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg;

namespace Features.Adventure.Player;

public class StartAdventureForPlayerTest
{
    [Theory]
    [InlineData(" Leading space", "Leading space")]
    [InlineData("Trailing space ", "Trailing space")]
    [InlineData(" Space on both ends ", "Space on both ends")]
    public void Success_trims_adventure_name(string adventureName, string trimmedAdventureName)
    {
        var command = CreateCommand(adventureName: adventureName);
        var data = CreateData();
        var result = StartAdventureForPlayer.ExecuteLogic(command, data);
        Assert.IsType<CreatedAtRoute>(result.Http);
        Assert.NotNull(result.SideEffects);
        Assert.Equal(trimmedAdventureName, result.SideEffects.CreateAdventure.Entity.Name);
    }

    [Theory]
    [InlineData(AdventureName.MinLength)]
    [InlineData(((AdventureName.MaxLength - AdventureName.MinLength) / 2) + AdventureName.MinLength)]
    [InlineData(AdventureName.MaxLength)]
    public void Succeeds_when_adventure_name_is_not_empty_and_does_not_exceed_max_length(int length)
    {
        var command = CreateCommand(adventureName: Generator.RandomString(length, includeSpace: false));
        var data = CreateData();
        var result = StartAdventureForPlayer.ExecuteLogic(command, data);
        AssertSuccess(result, command);
    }

    private static void AssertSuccess(
        FeatureResult<StartAdventureForPlayer.SideEffects?> result,
        StartAdventureForPlayer.Command command)
    {
        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<CreatedAtRoute>(result.Http);
                Assert.Multiple(
                    () => Assert.Equal(nameof(GetAdventureForPlayer), subject.RouteName),
                    () =>
                    {
                        var expected = new KeyValuePair<string, object?>("Id", command.AdventureId);
                        Assert.Contains(expected, subject.RouteValues);
                    });
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.NotNull(subject);
                Assert.Multiple(
                    () => Assert.IsType<CreateEntity<Wrpg.Adventure>>(subject.CreateAdventure),
                    () =>
                    {
                        var expected = new CreateEntity<Wrpg.Adventure>(AdventureGenerator.Create(
                            id: command.AdventureId,
                            userId: command.UserId,
                            characterId: command.CharacterId,
                            name: command.AdventureName,
                            status: AdventureStatus.Ongoing,
                            locationName: LocationName.Start));
                        Assert.Equivalent(expected, subject.CreateAdventure);
                    });
            });
    }

    [Fact]
    public void Fails_when_character_is_already_on_adventure()
    {
        var command = CreateCommand();
        var data = CreateData(characterIsAlreadyOnAdventure: true);
        var result = StartAdventureForPlayer.ExecuteLogic(command, data);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http);
            Assert.NotNull(subject.Value);
            Assert.Equal(StartAdventureForPlayer.CharacterIsAlreadyOnAdventureErrorMessage, subject.Value.Detail);
        });
    }

    [Theory]
    [InlineData(AdventureName.MaxLength + 1)]
    [InlineData(AdventureName.MaxLength + 2)]
    public void Fails_when_adventure_name_exceeds_max_length(int length)
    {
        var command = CreateCommand(adventureName: Generator.RandomString(length, includeSpace: false));
        var data = CreateData();
        var result = StartAdventureForPlayer.ExecuteLogic(command, data);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http);
            Assert.NotNull(subject.Value);
            Assert.Equal(AdventureName.MaxLengthErrorMessage, subject.Value.Detail);
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Fails_when_adventure_name_is_empty(string adventureName)
    {
        var command = CreateCommand(adventureName: adventureName);
        var data = CreateData();
        var result = StartAdventureForPlayer.ExecuteLogic(command, data);
        AssertFailure(result, () =>
        {
            var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http);
            Assert.NotNull(subject.Value);
            Assert.Equal(AdventureName.MinLengthErrorMessage, subject.Value.Detail);
        });
    }

    private static void AssertFailure(
        FeatureResult<StartAdventureForPlayer.SideEffects?> result,
        Action additionalTestCode)
    {
        Assert.Multiple(
            additionalTestCode,
            () => Assert.Null(result.SideEffects));
    }

    private static StartAdventureForPlayer.Command CreateCommand(
        Guid? adventureId = null,
        string? adventureName = null,
        Guid? characterId = null,
        string? userId = null) => new()
    {
        AdventureId = adventureId ?? Guid.NewGuid(),
        AdventureName = adventureName ?? AdventureName.Generate(),
        CharacterId = characterId ?? Guid.NewGuid(),
        UserId = userId ?? Generator.RandomString(UserId.MaxLength),
    };

    private static StartAdventureForPlayer.Data CreateData(
        string? characterName = null,
        bool characterIsAlreadyOnAdventure = false) => new()
    {
        CharacterName = characterName ?? Generator.RandomString(CharacterName.MaxLength),
        CharacterIsAlreadyOnAdventure = characterIsAlreadyOnAdventure,
    };
}