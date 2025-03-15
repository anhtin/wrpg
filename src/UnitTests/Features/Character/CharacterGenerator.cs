using Wrpg;

namespace Features.Character;

public static class CharacterGenerator
{
    public static Wrpg.Character Create(
        int id = 0,
        string? name = null,
        string? userId = null,
        Stats? stats = null) => new()
    {
        Id = id,
        UserId = userId ?? Generator.RandomString(),
        Name = name ?? Generator.RandomString(),
        Stats = stats ?? Stats.CreateNew(),
    };
}