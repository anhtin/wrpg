﻿using Features.Character;
using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;
using Wrpg.Shared.SideEffects;

namespace Features.Account;

public class DeleteAccountTest
{
    [Fact]
    public void Succeeds_when_Account_exists()
    {
        var data = CreateData(
            account: AccountGenerator.Create(),
            characters: [CharacterGenerator.Create(), CharacterGenerator.Create()]);

        var result = DeleteAccount.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<Ok>(result.Http.Result),
            () =>
            {
                var subject = result.SideEffects;
                Assert.NotNull(subject);
                Assert.Multiple(
                    () =>
                    {
                        var expected = new DeleteEntity<Wrpg.Account>(data.Account);
                        Assert.Equal(expected, subject.DeleteAccount);
                    },
                    () =>
                    {
                        var expected = data.Characters.Select(x => new DeleteEntity<Wrpg.Character>(x));
                        Assert.Equal(expected, subject.DeleteCharacters);
                    });
            });
    }
    
    [Fact]
    public void Fails_when_Account_does_not_exists()
    {
        DeleteAccount.Data? data = null;

        var result = DeleteAccount.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<NotFound>(result.Http.Result),
            () => Assert.Null(result.SideEffects));
    }

    private static DeleteAccount.Data CreateData(
        Wrpg.Account? account = null,
        IEnumerable<Wrpg.Character>? characters = null) => new()
    {
        Account = account ?? AccountGenerator.Create(),
        Characters = characters ?? [],
    };
}