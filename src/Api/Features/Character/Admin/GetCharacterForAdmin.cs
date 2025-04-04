﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Wrpg;

[Feature]
public static class GetCharacterForAdmin
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet("admin/character/{id}", Execute)
            .WithTags(EndpointTag.Role.Admin, EndpointTag.Resource.Character)
            .WithName(nameof(GetCharacterForAdmin))
            .RequirePermissionAny(Permission.CharacterReadAll, Permission.CharacterWriteAll);
    }

    internal static async Task<Results<Ok<Character>, NotFound>> Execute(Guid id, AppDbContext dbContext)
    {
        var character = await dbContext.Characters.SingleOrDefaultAsync(x => x.Id == id);
        return character is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Character>(new()
            {
                UserId = character.UserId,
                Name = character.Name,
                Stats = new()
                {
                    Attributes = new()
                    {
                        Level = character.Stats.Attributes.Level,
                        Strength = character.Stats.Attributes.Strength,
                        Dexterity = character.Stats.Attributes.Dexterity,
                        Intelligence = character.Stats.Attributes.Intelligence,
                        Constitution = character.Stats.Attributes.Constitution,
                        Spirit = character.Stats.Attributes.Spirit,
                    },
                    Resources = new()
                    {
                        Health = character.Stats.Resources.Health,
                        Energy = character.Stats.Resources.Energy,
                    },
                },
            });
    }

    public class Character
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required Stats Stats { get; init; }
    }

    public class Stats
    {
        public required Attributes Attributes { get; init; }
        public required Resources Resources { get; init; }
    }

    public class Attributes
    {
        public required int Level { get; init; }
        public required int Strength { get; init; }
        public required int Dexterity { get; init; }
        public required int Intelligence { get; init; }
        public required int Constitution { get; init; }
        public required int Spirit { get; init; }
    }

    public class Resources
    {
        public required int Health { get; init; }
        public required int Energy { get; init; }
    }
}