﻿using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared;
using Wrpg.Shared.Database;
using Wrpg.Shared.SideEffects;

namespace Wrpg;

[Feature]
public static class DeleteAccount
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("account/{nickname}", Execute)
            .WithTags(nameof(Account))
            .WithName(nameof(DeleteAccount));
    }

    internal static async Task<Results<Ok, NotFound>> Execute(string nickname, AppDbContext dbContext)
    {
        var command = new Command { Nickname = nickname };
        var data = await LoadData(command, dbContext);
        var result = ExecuteLogic(data);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    internal class Command
    {
        public required string Nickname { get; init; }
    }

    internal static async Task<Data?> LoadData(Command command, AppDbContext dbContext)
    {
        var query =
            from account in dbContext.Accounts
            join character in dbContext.Characters
                on account.Id equals character.AccountId
                into characters
            where account.Nickname == command.Nickname
            select new Data
            {
                Account = account,
                Characters = characters
            };

        return await query.FirstOrDefaultAsync();
    }

    internal class Data
    {
        public required Account Account { get; init; }
        public required IEnumerable<Character> Characters { get; init; }
    }

    internal static Result ExecuteLogic(Data? data)
    {
        return data is null
            ? new() { Http = TypedResults.NotFound() }
            : new()
            {
                Http = TypedResults.Ok(),
                SideEffects = new()
                {
                    DeleteAccount = new DeleteEntity<Account>(data.Account),
                    DeleteCharacters = data.Characters.Select(x => new DeleteEntity<Character>(x)),
                },
            };
    }

    internal class Result
    {
        public required Results<Ok, NotFound> Http { get; init; }
        public SideEffects? SideEffects { get; init; }
    }

    internal class SideEffects
    {
        public required DeleteEntity<Account> DeleteAccount { get; init; }
        public required IEnumerable<DeleteEntity<Character>> DeleteCharacters { get; init; }
    }

    internal static async Task ExecuteSideEffects(SideEffects? sideEffects, AppDbContext dbContext)
    {
        if (sideEffects is null) return;

        sideEffects.DeleteAccount.Execute(dbContext);
        foreach (var deleteCharacter in sideEffects.DeleteCharacters) deleteCharacter.Execute(dbContext);
        await dbContext.SaveChangesAsync();
    }
}