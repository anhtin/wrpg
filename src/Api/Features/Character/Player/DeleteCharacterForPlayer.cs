﻿using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Wrpg;

[Feature]
public static class DeleteCharacterForPlayer
{
    [UsedImplicitly]
    internal static void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("character/{id}", Execute)
            .WithTags(EndpointTag.Role.Player, EndpointTag.Resource.Character)
            .WithName(nameof(DeleteCharacterForPlayer))
            .RequirePermissionAny(Permission.CharacterWriteOwn)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    internal static async Task<IResult> Execute(
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
        return await FeatureHelper.Execute(
            () => LoadData(command, dbContext),
            ExecuteLogic,
            sideEffects => ExecuteSideEffects(sideEffects, dbContext));
    }

    internal class Command
    {
        public required Guid CharacterId { get; init; }
        public required string UserId { get; init; }
    }

    internal static async Task<Data> LoadData(Command command, AppDbContext dbContext)
    {
        var character = await dbContext.Characters
            .SingleOrDefaultAsync(x => x.Id == command.CharacterId && x.UserId == command.UserId);

        return new() { Character = character };
    }

    internal class Data
    {
        public required Character? Character { get; init; }
    }

    internal static FeatureResult<SideEffects?> ExecuteLogic(Data data)
    {
        if (data.Character is null)
            return TypedResults.NotFound();

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