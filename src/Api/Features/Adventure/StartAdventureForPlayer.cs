using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Wrpg;

using HttpResult = Results<CreatedAtRoute, NotFound, BadRequest<ProblemDetails>, Conflict>;

[Feature]
public static class StartAdventureForPlayer
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("adventure", Execute)
            .WithTags(EndpointTag.Role.Player, EndpointTag.Resource.Adventure)
            .WithName(nameof(StartAdventureForPlayer))
            .RequirePermissionAny(Permission.CharacterWriteOwn);
    }

    internal static async Task<HttpResult> Execute(
        [FromHeader(Name = CustomHttpHeader.IdempotencyKey)]
        Guid idempotencyKey,
        [FromBody] Request request,
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var command = new Command
        {
            AdventureId = idempotencyKey,
            AdventureName = request.AdventureName,
            CharacterId = request.CharacterId,
            UserId = userId,
        };
        return await FeatureHelper.TryExecute(
            () => LoadData(command, dbContext),
            data => ExecuteLogic(command, data),
            sideEffects => ExecuteSideEffects(sideEffects, dbContext),
            e => HandleException(e, command, dbContext));
    }

    public class Request
    {
        public required Guid CharacterId { get; init; }
        public required string? AdventureName { get; init; }
    }

    public class Command
    {
        public required Guid AdventureId { get; init; }
        public required string? AdventureName { get; init; }
        public required Guid CharacterId { get; init; }
        public required string UserId { get; init; }
    }

    internal static async Task<Data?> LoadData(Command command, AppDbContext dbContext)
    {
        var characterName = await dbContext.Characters
            .Where(x => x.Id == command.CharacterId && x.UserId == command.UserId)
            .Select(x => x.Name)
            .SingleOrDefaultAsync();

        if (characterName is null)
            return null;

        var isAlreadyOnAdventure = await dbContext.Adventures
            .AnyAsync(x => x.CharacterId == command.CharacterId && x.Status != AdventureStatus.Ended);

        return new()
        {
            CharacterName = characterName,
            CharacterIsAlreadyOnAdventure = isAlreadyOnAdventure
        };
    }

    internal class Data
    {
        public required string CharacterName { get; init; }
        public required bool CharacterIsAlreadyOnAdventure { get; init; }
    }

    internal static FeatureResult<HttpResult, SideEffects?> ExecuteLogic(Command command, Data? data)
    {
        if (data is null) return new() { Http = TypedResults.NotFound() };

        if (data.CharacterIsAlreadyOnAdventure)
            return new() { Http = CustomTypedResults.BadRequest(CharacterIsAlreadyOnAdventureErrorMessage) };

        var adventureName = AdventureName.Normalized(command.AdventureName ?? AdventureName.Generate());
        if (!AdventureName.IsValid(adventureName, out var error))
            return new() { Http = CustomTypedResults.BadRequest(error) };

        var adventure = Adventure.CreateNew(command.AdventureId, command.UserId, command.CharacterId, adventureName);

        return new()
        {
            Http = TypedResults.CreatedAtRoute(nameof(GetAdventureForPlayer), new { Id = adventure.Id }),
            SideEffects = new()
            {
                CreateAdventure = new CreateEntity<Adventure>(adventure),
            },
        };
    }

    internal const string CharacterIsAlreadyOnAdventureErrorMessage = "Character is already on adventure";

    internal class SideEffects
    {
        public required CreateEntity<Adventure> CreateAdventure { get; init; }
    }

    internal static async Task ExecuteSideEffects(SideEffects? sideEffects, AppDbContext dbContext)
    {
        if (sideEffects is null) return;

        sideEffects.CreateAdventure.Execute(dbContext);
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
                var hasMatch = await dbContext.Adventures
                    .AnyAsync(x => x.Id == command.AdventureId &&
                                   (command.AdventureName == null || x.Name == command.AdventureName));

                return hasMatch
                    ? TypedResults.CreatedAtRoute(nameof(GetAdventureForPlayer), new { Id = command.AdventureId })
                    : TypedResults.Conflict();
            }
        }

        return null;
    }
}