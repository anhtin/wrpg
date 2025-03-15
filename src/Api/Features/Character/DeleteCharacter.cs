using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Wrpg;

using HttpResult = Results<Ok, NotFound>;

[Feature]
public static class DeleteCharacter
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("character/{id}", Execute)
            .WithTags(nameof(Character))
            .WithName(nameof(DeleteCharacter));
    }

    internal static async Task<HttpResult> Execute(
        Guid id,
        ClaimsPrincipal user,
        AppDbContext dbContext)
    {
        var userId = UserId.ResolveFrom(user);
        var command = new Command
        {
            CharacterId = id,
            UserId = userId,
        };
        var data = await LoadData(command, dbContext);
        var result = ExecuteLogic(data);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    internal class Command
    {
        public required Guid CharacterId { get; init; }
        public required string UserId { get; init; }
    }

    internal class Data
    {
        public required Character? Character { get; init; }
    }

    internal static async Task<Data> LoadData(Command command, AppDbContext dbContext)
    {
        var character = await dbContext.Characters
            .SingleOrDefaultAsync(x => x.Id == command.CharacterId && x.UserId == command.UserId);

        return new() { Character = character };
    }

    internal static Result ExecuteLogic(Data data)
    {
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

    internal class Result
    {
        public required HttpResult Http { get; init; }
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