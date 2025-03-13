namespace Wrpg;

public class SwaggerOptions
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }

    public static SwaggerOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("Swagger").Get<SwaggerOptions>()!;
    }
}