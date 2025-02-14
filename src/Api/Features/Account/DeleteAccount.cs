using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared.Database;
using Wrpg.Shared.SideEffects;

namespace Wrpg;

public static class DeleteAccount
{
    public static TEndpointRouteBuilder MapDeleteAccountEndpoint<TEndpointRouteBuilder>(
        this TEndpointRouteBuilder builder)
        where TEndpointRouteBuilder : IEndpointRouteBuilder
    {
        builder.MapDelete("account/{nickname}", Execute)
            .WithTags(nameof(Account))
            .WithName(nameof(DeleteAccount));
        return builder;
    }

    internal static async Task<Results<Ok, NotFound, BadRequest<ProblemDetails>>> Execute(
        string nickname,
        AppDbContext dbContext)
    {
        var command = new Command { Nickname = nickname };
        var data = await LoadData(command, dbContext);
        var result = ExecuteLogic(data);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    internal class Command
    {
        public required string Nickname { get; set; }
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
        public required Account Account { get; set; }
        public required IEnumerable<Character> Characters { get; set; }
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
        public required Results<Ok, NotFound, BadRequest<ProblemDetails>> Http { get; set; }
        public SideEffects? SideEffects { get; set; }
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