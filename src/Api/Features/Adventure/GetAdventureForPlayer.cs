using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Wrpg;

[Feature]
public static class GetAdventureForPlayer
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet("adventure/{id}", Execute)
            .WithTags(EndpointTag.Role.Player, EndpointTag.Resource.Adventure)
            .WithName(nameof(GetAdventureForPlayer))
            .RequirePermissionAny(Permission.CharacterReadOwn, Permission.CharacterWriteOwn);
    }

    internal static async Task<Results<Ok<Adventure>, NotFound>> Execute(
        Guid id,
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var adventure = await dbContext.Adventures.SingleOrDefaultAsync(x => x.UserId == userId && x.Id == id);
        return adventure is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Adventure>(new()
            {
                CharacterId = adventure.CharacterId,
                Name = adventure.Name,
                Status = adventure.Status,
                LocationName = adventure.LocationName,
            });
    }

    public class Adventure
    {
        public required Guid CharacterId { get; init; }
        public required string Name { get; init; }
        public required AdventureStatus Status { get; init; }
        public required string LocationName { get; init; }
    }
}