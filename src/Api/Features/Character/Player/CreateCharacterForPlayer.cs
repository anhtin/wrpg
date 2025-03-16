using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Wrpg;

using HttpResult = Results<CreatedAtRoute, Conflict, BadRequest<ProblemDetails>>;

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
        return await FeatureHelper.TryExecute(
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

    internal static FeatureResult<HttpResult, SideEffects?> ExecuteLogic(Command command)
    {
        if (string.IsNullOrWhiteSpace(command.CharacterName))
            return new() { Http = CustomTypedResults.BadRequest(CharacterNameIsEmptyMessage) };
        if (command.CharacterName.Length > Character.MaxNameLength)
            return new() { Http = CustomTypedResults.BadRequest(CharacterNameExceedsMaxLengthMessage) };

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

    internal const string CharacterNameIsEmptyMessage = "Character name cannot be empty";

    internal static readonly string CharacterNameExceedsMaxLengthMessage =
        $"Character name cannot exceed {Character.MaxNameLength} characters";

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