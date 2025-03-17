using Wrpg;

namespace Features.Adventure;

public static class AdventureGenerator
{
    public static Wrpg.Adventure Create(
        Guid? id = null,
        int internalId = 0,
        string? userId = null,
        Guid? characterId = null,
        string? name = null,
        AdventureStatus? status = null,
        string? locationName = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        InternalId = internalId,
        UserId = userId ?? Generator.RandomString(),
        CharacterId = characterId ?? Guid.NewGuid(),
        Name = name ?? Generator.RandomString(),
        Status = status ?? AdventureStatus.Ended,
        LocationName = locationName ?? LocationName.Start,
    };
}