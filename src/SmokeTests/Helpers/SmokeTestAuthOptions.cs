using Microsoft.Extensions.Configuration;

namespace Helpers;

public class SmokeTestAuthOptions
{
    public required SmokeTestAuthOptionsTokens AccessTokens { get; init; }

    public static SmokeTestAuthOptions CreateFrom(IConfigurationRoot configurationRoot)
    {
        return configurationRoot.GetRequiredSection("Tests:SmokeTest:Auth").Get<SmokeTestAuthOptions>()!;
    }
}

public class SmokeTestAuthOptionsTokens
{
    public required string Admin { get; init; }
    public required string Player1 { get; init; }
    public required string Player2 { get; init; }
}