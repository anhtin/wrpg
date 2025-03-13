namespace Wrpg;

public class OauthOptions
{
    public required string Authority { get; init; }
    public required string Audience { get; init; }

    public static OauthOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("OAuth").Get<OauthOptions>()!;
    }
}