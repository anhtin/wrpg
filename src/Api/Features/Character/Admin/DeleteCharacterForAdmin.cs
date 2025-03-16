using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Wrpg;

using HttpResult = Results<Ok, NotFound>;

[Feature]
public static class DeleteCharacterForAdmin
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("admin/character/{id}", Execute)
            .WithTags(EndpointTag.Role.Admin, EndpointTag.Resource.Character)
            .WithName(nameof(DeleteCharacterForAdmin))
            .RequirePermissionAny(Permission.CharacterWriteAll);
    }

    internal static async Task<HttpResult> Execute(Guid id, AppDbContext dbContext)
    {
        var command = new Command
        {
            CharacterId = id,
        };
        return await FeatureHelper.Execute(
            () => LoadData(command, dbContext),
            ExecuteLogic,
            sideEffects => ExecuteSideEffects(sideEffects, dbContext));
    }

    internal class Command
    {
        public required Guid CharacterId { get; init; }
    }

    internal class Data
    {
        public required Character? Character { get; init; }
    }

    internal static async Task<Data> LoadData(Command command, AppDbContext dbContext)
    {
        var character = await dbContext.Characters.SingleOrDefaultAsync(x => x.Id == command.CharacterId);
        return new() { Character = character };
    }

    internal static FeatureResult<HttpResult, SideEffects?> ExecuteLogic(Data data)
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