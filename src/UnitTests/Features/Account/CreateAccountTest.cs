using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg.Shared.SideEffects;

namespace Wrpg.UnitTests;

public class CreateAccountTest
{
    [Fact]
    public void Succeeds_when_all_is_good()
    {
        var command = CreateCommand(
            identityProvider: Generator.RandomString(),
            identityId: Generator.RandomString(),
            nickname: Generator.RandomString());

        var result = CreateAccount.ExecuteLogic(command);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<CreatedAtRoute>(result.Http.Result);
                Assert.Multiple(
                    () => Assert.Equal(nameof(GetAccount), subject.RouteName),
                    () =>
                    {
                        var expected = new KeyValuePair<string, object?>("Nickname", command.Nickname);
                        Assert.Contains(expected, subject.RouteValues);
                    });
            },
            () =>
            {
                Assert.NotNull(result.SideEffects);
                var subject = result.SideEffects.CreateAccount;
                Assert.Multiple(
                    () => Assert.IsType<CreateEntity<Account>>(subject),
                    () =>
                    {
                        var expected = new CreateEntity<Account>(AccountGenerator.Create(
                            identityProvider: command.IdentityProvider,
                            identityId: command.IdentityId,
                            nickname: command.Nickname));
                        Assert.Equivalent(expected, subject);
                    });
            });
    }

    [Fact]
    public void Fails_when_nickname_is_invalid()
    {
        var nickname = "Invalid nickname";
        Assert.False(Nickname.IsValid(nickname));

        var command = CreateCommand(nickname: nickname);

        var result = CreateAccount.ExecuteLogic(command);

        Assert.Multiple(
            () =>
            {
                var subject = Assert.IsType<BadRequest<ProblemDetails>>(result.Http.Result);
                var expected = CreateAccount.BadNicknameMessage;
                Assert.NotNull(subject.Value);
                Assert.Equal(expected, subject.Value.Detail);
            },
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    private static CreateAccount.Command CreateCommand(
        string? identityProvider = null,
        string? identityId = null,
        string? nickname = null) => new()
    {
        IdentityProvider = identityProvider ?? Generator.RandomString(),
        IdentityId = identityId ?? Generator.RandomString(),
        Nickname = nickname ?? Generator.RandomString(),
    };
}