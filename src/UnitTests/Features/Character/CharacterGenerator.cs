using Wrpg;

namespace Features.Character;

public static class CharacterGenerator
{
    public static Wrpg.Character Create(
        Guid? id = null,
        int internalId = 0,
        string? userId = null,
        string? name = null,
        Stats? stats = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        InternalId = internalId,
        UserId = userId ?? Generator.RandomString(),
        Name = name ?? Generator.RandomString(),
        Stats = stats ?? Stats.CreateNew(),
    };
}