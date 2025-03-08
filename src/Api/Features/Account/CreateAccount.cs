using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg.Shared;
using Wrpg.Shared.Database;
using Wrpg.Shared.SideEffects;

namespace Wrpg;

[Feature]
public static class CreateAccount
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapPost("account", Execute)
            .WithTags(nameof(Account))
            .WithName(nameof(CreateAccount));
    }

    internal static async Task<Results<CreatedAtRoute, BadRequest<ProblemDetails>>> Execute(
        [FromBody] Request body,
        AppDbContext dbContext)
    {
        var command = new Command
        {
            IdentityProvider = body.IdentityProvider,
            IdentityId = body.IdentityId,
            Nickname = body.Nickname,
        };
        var result = ExecuteLogic(command);
        await ExecuteSideEffects(result.SideEffects, dbContext);
        return result.Http;
    }

    public class Request
    {
        public required string IdentityProvider { get; init; }
        public required string IdentityId { get; init; }
        public required string Nickname { get; init; }
    }

    internal class Command
    {
        public required string IdentityProvider { get; init; }
        public required string IdentityId { get; init; }
        public required string Nickname { get; init; }
    }

    internal static Result ExecuteLogic(Command command)
    {
        var normalizedNickname = Nickname.Normalize(command.Nickname);
        if (!Nickname.IsValid(normalizedNickname))
            return new() { Http = TypedResults.BadRequest(new ProblemDetails { Detail = BadNicknameMessage }) };

        var account = Account.CreateNew(command.IdentityProvider, command.IdentityId, normalizedNickname);
        return new()
        {
            Http = TypedResults.CreatedAtRoute(nameof(GetAccount), new { Nickname = account.Nickname }),
            SideEffects = new()
            {
                CreateAccount = new CreateEntity<Account>(account),
            },
        };
    }

    internal const string BadNicknameMessage = $"Nickname must match pattern '{Nickname.Pattern}'";

    internal class Result
    {
        public required Results<CreatedAtRoute, BadRequest<ProblemDetails>> Http { get; init; }
        public SideEffects? SideEffects { get; init; }
    }

    internal class SideEffects
    {
        public required CreateEntity<Account> CreateAccount { get; init; }
    }

    internal static async Task ExecuteSideEffects(SideEffects? sideEffects, AppDbContext dbContext)
    {
        if (sideEffects is null) return;

        sideEffects.CreateAccount.Execute(dbContext);
        await dbContext.SaveChangesAsync();
    }
}