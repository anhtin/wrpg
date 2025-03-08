using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared;
using Wrpg.Shared.Database;
using Wrpg.Shared.SideEffects;

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
        AppDbContext dbContext)
    {
        var command = new Command
        {
            CharacterName = body.CharacterName,
            AccountNickname = body.AccountNickname,
        };
        var data = await LoadData(command, dbContext);
        var result = ExecuteLogic(command, data);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    public class Request
    {
        public required string CharacterName { get; init; }
        public required string AccountNickname { get; init; }
    }

    internal class Command
    {
        public required string CharacterName { get; init; }
        public required string AccountNickname { get; init; }
    }

    internal static async Task<Data> LoadData(Command command, AppDbContext dbContext)
    {
        var accountId = await dbContext.Accounts
            .Where(x => x.Nickname == command.AccountNickname)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync();
        return new()
        {
            AccountId = accountId,
        };
    }

    public class Data
    {
        public required int? AccountId { get; init; }
    }

    internal static Result ExecuteLogic(Command command, Data data)
    {
        if (!data.AccountId.HasValue)
            return new() { Http = TypedResults.BadRequest(new ProblemDetails { Detail = MissingAccountMessage }) };

        var normalizedName = CharacterName.Normalize(command.CharacterName);
        if (!CharacterName.IsValid(normalizedName))
            return new() { Http = TypedResults.BadRequest(new ProblemDetails { Detail = BadCharacterNameMessage }) };

        var character = Character.CreateNew(normalizedName, data.AccountId.Value);
        return new()
        {
            Http = TypedResults.CreatedAtRoute(nameof(GetCharacter), new { Name = character.Name }),
            SideEffects = new()
            {
                CreateCharacter = new CreateEntity<Character>(character),
            },
        };
    }

    internal const string MissingAccountMessage = "Account does not exist";
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