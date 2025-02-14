using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg.Shared.SideEffects;

namespace Wrpg.UnitTests;

public class DeleteAccountTest
{
    [Fact]
    public void Returns_200_Ok_when_Data_is_not_null()
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
                        var expected = new DeleteEntity<Account>(data.Account);
                        Assert.Equal(expected, subject.DeleteAccount);
                    },
                    () =>
                    {
                        var expected = data.Characters.Select(x => new DeleteEntity<Character>(x));
                        Assert.Equal(expected, subject.DeleteCharacters);
                    });
            });
    }
    
    [Fact]
    public void Returns_400_Not_Found_when_Data_is_not_null()
    {
        DeleteAccount.Data? data = null;

        var result = DeleteAccount.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<NotFound>(result.Http.Result),
            () => Assert.Null(result.SideEffects));
    }

    private static DeleteAccount.Data CreateData(
        Account? account = null,
        IEnumerable<Character>? characters = null) => new()
    {
        Account = account ?? AccountGenerator.Create(),
        Characters = characters ?? [],
    };
}