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
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var command = new Command
        {
            UserId = userId,
            CharacterName = body.CharacterName,
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
        public required string UserId { get; init; }
        public required string CharacterName { get; init; }
    }

    internal static Result ExecuteLogic(Command command)
    {
        var normalizedName = CharacterName.Normalize(command.CharacterName);
        if (!CharacterName.IsValid(normalizedName))
            return new() { Http = CustomTypedResults.BadRequest(BadCharacterNameMessage) };

        var character = Character.CreateNew(normalizedName, command.UserId);
        return new()
        {
            Http = TypedResults.CreatedAtRoute(nameof(GetCharacter), new { Name = character.Name }),
            SideEffects = new()
            {
                CreateCharacter = new CreateEntity<Character>(character),
            },
        };
    }

    internal const string BadCharacterNameMessage = $"Character name must match '{CharacterName.Pattern}'";

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