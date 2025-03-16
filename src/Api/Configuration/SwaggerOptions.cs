namespace Wrpg;

public class SwaggerOptions
{
    public required OAuthClientCredentials Credentials { get; init; }

    public static SwaggerOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("Swagger").Get<SwaggerOptions>()!;
    }
}