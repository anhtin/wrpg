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
}