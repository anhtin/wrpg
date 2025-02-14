using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg.Shared.SideEffects;

namespace Wrpg.UnitTests;

public class CreateCharacterTest
{
    [Fact]
    public void Returns_200_Ok_when_all_is_good()
    {
        var command = CreateCommand(characterName: Generator.RandomString());
        var data = CreateData(accountId: Generator.RandomInt());

        var result = CreateCharacter.ExecuteLogic(command, data);

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
                    () => Assert.IsType<CreateEntity<Character>>(subject.CreateCharacter),
                    () =>
                    {
                        var expected = new CreateEntity<Character>(CharacterGenerator.Create(
                            name: command.CharacterName,
                            accountId: data.AccountId,
                            stats: Stats.CreateNew()));
                        Assert.Equivalent(expected, subject.CreateCharacter);
                    });
            });
    }

    [Fact]
    public void Returns_400_Bad_Request_when_AccountId_is_null()
    {
        var command = CreateCommand();
        var data = CreateData(accountId: null);

        var result = CreateCharacter.ExecuteLogic(command, data);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
                var expected = CreateCharacter.MissingAccountMessage;
                Assert.NotNull(subject.Value);
                Assert.Equal(expected, subject.Value.Detail);
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    [Theory]
    [InlineData("Character name with space")]
    [InlineData("$pecial $ymbols")]
    public void Returns_400_Bad_Request_when_CharacterName_is_bad(string characterName)
    {
        var command = CreateCommand(characterName: characterName);
        var data = CreateData();

        var result = CreateCharacter.ExecuteLogic(command, data);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
                var expected = CreateCharacter.MissingAccountMessage;
                Assert.NotNull(subject.Value);
                Assert.Equal(expected, subject.Value.Detail);
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    private static CreateCharacter.Command CreateCommand(string? characterName = null) => new()
    {
        CharacterName = characterName ?? Generator.RandomString(),
        AccountNickname = null!,
    };

    private static CreateCharacter.Data CreateData(int? accountId = null) => new()
    {
        AccountId = accountId,
    };
}