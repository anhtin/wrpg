using Microsoft.EntityFrameworkCore;

namespace Wrpg;

public static class CharacterExtensions
{
    public static TEndpointRouteBuilder MapCharacterEndpoints<TEndpointRouteBuilder>(this TEndpointRouteBuilder builder)
        where TEndpointRouteBuilder : IEndpointRouteBuilder
    {
        return builder
            .MapCreateCharacterEndpoint()
            .MapGetCharacterEndpoint();
    }

    public static ModelBuilder AddCharacterEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(Character.Configure);
        return modelBuilder;
    }
}