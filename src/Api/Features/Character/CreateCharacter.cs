using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Wrpg;

[Feature]
public static class CreateCharacter
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("character", Execute)
            .WithTags(nameof(Character))
            .WithName(nameof(CreateCharacter));
    }

    internal static async Task<Results<CreatedAtRoute, BadRequest<ProblemDetails>>> Execute(
        [FromBody] Request body,
        [FromHeader(Name = CustomHttpHeader.IdempotencyKey)]
        Guid idempotencyKey,
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var command = new Command
        {
            CharacterId = idempotencyKey,
            CharacterName = body.CharacterName,
            UserId = userId,
        };
        var result = ExecuteLogic(command);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    public class Request
    {
        public required string CharacterName { get; init; }
    }

    internal class Command
    {
        public required Guid CharacterId { get; init; }
        public required string CharacterName { get; init; }
        public required string UserId { get; init; }
    }

    internal static Result ExecuteLogic(Command command)
    {
        var character = Character.CreateNew(command.CharacterId, command.CharacterName, command.UserId);
        return new()
        {
            Http = TypedResults.CreatedAtRoute(nameof(GetCharacter), new { Id = character.Id }),
            SideEffects = new()
            {
                CreateCharacter = new CreateEntity<Character>(character),
            },
        };
    }

    internal class Result
    {
        public required Results<CreatedAtRoute, BadRequest<ProblemDetails>> Http { get; init; }
        public SideEffects? SideEffects { get; init; }
    }

    internal class SideEffects
    {
        public required CreateEntity<Character> CreateCharacter { get; init; }
    }

    internal static async Task ExecuteSideEffects(SideEffects? sideEffects, AppDbContext dbContext)
    {
        if (sideEffects is null) return;

        sideEffects.CreateCharacter.Execute(dbContext);
        await dbContext.SaveChangesAsync();
    }
}