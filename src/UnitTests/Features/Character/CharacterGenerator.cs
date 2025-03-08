using Wrpg;

namespace Features.Character;

public static class CharacterGenerator
{
    public static Wrpg.Character Create(
        int id = 0,
        string? name = null,
        int? accountId = null,
        Stats? stats = null) => new()
    {
        Id = id,
        Name = name ?? Generator.RandomString(),
        AccountId = accountId ?? Generator.RandomInt(),
        Stats = stats ?? Stats.CreateNew(),
    };
}