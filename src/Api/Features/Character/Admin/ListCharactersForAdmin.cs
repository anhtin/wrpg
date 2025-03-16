using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Wrpg;

[Feature]
public static class ListCharactersForAdmin
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("admin/character", Execute)
            .WithTags(EndpointTag.Role.Admin, EndpointTag.Resource.Character)
            .WithName(nameof(ListCharactersForAdmin))
            .RequirePermissionAny(Permission.CharacterReadAll);
    }

    internal static async Task<Results<Ok<Page<Character>>, BadRequest<ProblemDetails>>> Execute(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        AppDbContext dbContext)
    {
        pageNumber ??= 1;
        pageSize ??= 100;

        if (pageNumber.Value <= 0) return CustomTypedResults.BadRequest(InvalidPageNumberMessage);
        if (pageSize.Value <= 0) return CustomTypedResults.BadRequest(InvalidPageSizeMessage);

        var page = await dbContext.Characters
            .OrderBy(x => x.InternalId)
            .Select(x => new Character
            {
                Id = x.Id,
                Name = x.Name,
                Stats = new Stats
                {
                    Attributes = new()
                    {
                        Level = x.Stats.Attributes.Level,
                        Strength = x.Stats.Attributes.Strength,
                        Dexterity = x.Stats.Attributes.Dexterity,
                        Intelligence = x.Stats.Attributes.Intelligence,
                        Constitution = x.Stats.Attributes.Constitution,
                        Spirit = x.Stats.Attributes.Spirit,
                    },
                    Resources = new()
                    {
                        Health = x.Stats.Resources.Health,
                        Energy = x.Stats.Resources.Energy,
                    },
                },
            })
            .ToPageAsync(pageNumber.Value, pageSize.Value);

        return TypedResults.Ok(page);
    }

    internal const string InvalidPageNumberMessage = "Page number must be greater than zero";
    internal const string InvalidPageSizeMessage = "Page size must be greater than zero";

    public class Character
    {
        public required Guid Id { get; init; }
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