﻿using Microsoft.AspNetCore.Http.HttpResults;
using Wrpg;

namespace Features.Character;

public class DeleteCharacterForPlayerTest
{
    [Fact]
    public void Succeeds_when_all_is_good()
    {
        var data = CreateData(character: CharacterGenerator.Create());
        var result = DeleteCharacterForPlayer.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<Ok>(result.Http.Result),
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
        var result = DeleteCharacterForPlayer.ExecuteLogic(data);

        Assert.Multiple(
            () => Assert.IsType<NotFound>(result.Http.Result),
            () =>
            {
                var subject = result.SideEffects;
                Assert.Null(subject);
            });
    }

    private static DeleteCharacterForPlayer.Data CreateData(Wrpg.Character? character) => new()
    {
        Character = character,
    };
}