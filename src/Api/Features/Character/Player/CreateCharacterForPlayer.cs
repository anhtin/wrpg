using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Wrpg;

using HttpResult = Results<CreatedAtRoute, Conflict>;

[Feature]
public static class CreateCharacterForPlayer
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("character", Execute)
            .WithTags(EndpointTag.Role.Player, EndpointTag.Resource.Character)
            .WithName(nameof(CreateCharacterForPlayer))
            .RequirePermissionAny(Permission.CharacterWriteOwn);
    }

    internal static async Task<HttpResult> Execute(
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
        return await FeatureHelper.TryExecute<Result, HttpResult, SideEffects?>(
            () => ExecuteLogic(command),
            sideEffects => ExecuteSideEffects(sideEffects, dbContext),
            e => HandleException(e, command, dbContext));
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
            Http = TypedResults.CreatedAtRoute(nameof(GetCharacterForPlayer), new { Id = character.Id }),
            SideEffects = new()
            {
                CreateCharacter = new CreateEntity<Character>(character),
            },
        };
    }

    internal class Result : FeatureResult<HttpResult, SideEffects?>;

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

    internal static async Task<HttpResult?> HandleException(
        Exception e,
        Command command,
        AppDbContext dbContext)
    {
        switch (e)
        {
            case DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            }:
            {
                var characterId = await dbContext.Characters
                    .Where(x => x.Id == command.CharacterId && x.Name == command.CharacterName)
                    .SingleOrDefaultAsync();

                return characterId is null
                    ? TypedResults.Conflict()
                    : TypedResults.CreatedAtRoute(nameof(GetCharacterForPlayer), new { Id = characterId });
            }
        }

        return null;
    }
}