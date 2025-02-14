using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wrpg.Shared.Database;

namespace Wrpg;

public static class GetAccount
{
    public class Response
    {
        public required string Nickname { get; init; }
    }

    public static TEndpointRouteBuilder MapGetAccountEndpoint<TEndpointRouteBuilder>(
        this TEndpointRouteBuilder builder)
        where TEndpointRouteBuilder : IEndpointRouteBuilder
    {
        builder.MapGet("account/{nickname}", Execute)
            .WithTags(nameof(Account))
            .WithName(nameof(GetAccount));
        return builder;
    }

    public static async Task<Results<Ok<Response>, NotFound>> Execute(string nickname, AppDbContext dbContext)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Nickname == nickname);
        return account is null
            ? TypedResults.NotFound()
            : TypedResults.Ok<Response>(new()
            {
                Nickname = account.Nickname,
            });
    }
}