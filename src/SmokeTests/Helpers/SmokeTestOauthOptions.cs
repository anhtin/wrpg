using Microsoft.Extensions.Configuration;

namespace Helpers;

public class SmokeTestOauthOptions
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }

    public static SmokeTestOauthOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("SmokeTest:OAuth").Get<SmokeTestOauthOptions>()!;
    }
}