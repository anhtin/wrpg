using Microsoft.Extensions.Configuration;
using Wrpg;

namespace Helpers;

public class SmokeTestOauthOptions
{
    public required SmokeTestOauthOptionsCredentials Credentials { get; init; }

    public static SmokeTestOauthOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("SmokeTest:OAuth").Get<SmokeTestOauthOptions>()!;
    }
}

public class SmokeTestOauthOptionsCredentials
{
    public required OAuthClientCredentials Admin { get; init; }
    public required OAuthClientCredentials Player1 { get; init; }
    public required OAuthClientCredentials Player2 { get; init; }
}