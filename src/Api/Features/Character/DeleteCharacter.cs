using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared;
using Wrpg.Shared.Database;
using Wrpg.Shared.SideEffects;

namespace Wrpg;

[Feature]
public static class DeleteCharacter
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("character/{name}", Execute)
            .WithTags(nameof(Character))
            .WithName(nameof(DeleteCharacter));
    }

    internal static async Task<Results<Ok, BadRequest<ProblemDetails>, NotFound>> Execute(
        string name,
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var command = new Command
        {
            UserId = userId,
            CharacterName = name,
        };
        var data = await LoadData(command, dbContext);
        var result = ExecuteLogic(command, data);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    internal class Command
    {
        public required string UserId { get; init; }
        public required string CharacterName { get; init; }
    }

    internal class Data
    {
        public required Character? Character { get; init; }
    }

    internal static async Task<Data> LoadData(Command command, AppDbContext dbContext)
    {
        var character = await dbContext.Characters
            .SingleOrDefaultAsync(x => x.UserId == command.UserId && x.Name == command.CharacterName);

        return new() { Character = character };
    }

    internal static Result ExecuteLogic(Command command, Data data)
    {
        var normalizedName = CharacterName.Normalize(command.CharacterName);
        if (!CharacterName.IsValid(normalizedName))
            return new() { Http = CustomTypedResults.BadRequest(BadCharacterNameMessage) };

        if (data.Character is null)
            return new() { Http = TypedResults.NotFound() };

        return new()
        {
            Http = TypedResults.Ok(),
            SideEffects = new()
            {
                DeleteCharacter = new DeleteEntity<Character>(data.Character),
            },
        };
    }

    internal const string BadCharacterNameMessage = $"Character name must match '{CharacterName.Pattern}'";

    internal class Result
    {
        public required Results<Ok, BadRequest<ProblemDetails>, NotFound> Http { get; init; }
        public SideEffects? SideEffects { get; init; }
    }

    internal class SideEffects
    {
        public required DeleteEntity<Character> DeleteCharacter { get; init; }
    }

    internal static async Task ExecuteSideEffects(SideEffects? sideEffects, AppDbContext dbContext)
    {
        if (sideEffects is null) return;

        sideEffects.DeleteCharacter.Execute(dbContext);
        await dbContext.SaveChangesAsync();
    }
}