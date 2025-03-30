using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;

namespace Features.Character.Admin;

public class DeleteCharacterForAdminTest
{
    [Fact]
    public void Succeeds_when_character_exists()
    {
        var data = CreateData(character: CharacterGenerator.Create());
        var result = DeleteCharacterForAdmin.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<Ok>(result.Http),
            () =>
            {
                var subject = result.SideEffects;
                Assert.NotNull(subject);
                Assert.IsType<DeleteEntity<Wrpg.Character>>(subject.DeleteCharacter);
            });
    }

    [Fact]
    public void Fails_when_character_does_not_exist()
    {
        var data = CreateData(character: null);
        var result = DeleteCharacterForAdmin.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<NotFound>(result.Http),
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    private static DeleteCharacterForAdmin.Data CreateData(Wrpg.Character? character) => new()
    {
        Character = character,
    };
}