using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared;
using Wrpg.Shared.Database;

namespace Wrpg;

[Feature]
public static class GetAccount
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapGet("account/{nickname}", Execute)
            .WithTags(nameof(Account))
            .WithName(nameof(GetAccount));
    }

    internal static async Task<Results<Ok<Response>, NotFound>> Execute(string nickname, AppDbContext dbContext)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Nickname == nickname);
        return account is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Response>(new()
            {
                Nickname = account.Nickname,
            });
    }

    public class Response
    {
        public required string Nickname { get; init; }
    }
}