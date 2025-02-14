using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared.Database;

namespace Wrpg;

public static class GetCharacter
{
    public class Response
    {
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

    public static TEndpointRouteBuilder MapGetCharacterEndpoint<TEndpointRouteBuilder>(
        this TEndpointRouteBuilder builder)
        where TEndpointRouteBuilder : IEndpointRouteBuilder
    {
        builder.MapGet("character/{name}", Execute)
            .WithTags(nameof(Character))
            .WithName(nameof(GetCharacter));
        return builder;
    }

    public static async Task<Results<Ok<Response>, NotFound>> Execute(string name, AppDbContext dbContext)
    {
        var character = await dbContext.Characters.FirstOrDefaultAsync(x => x.Name == name);
        return character is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Response>(new()
            {
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
}