using Microsoft.AspNetCore.Authorization;

namespace Wrpg;

public static class AuthorizationConfiguration
{
    public static void Configure(AuthorizationOptions options)
    {
        options.FallbackPolicy = options.DefaultPolicy;
    }
}