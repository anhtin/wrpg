using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Wrpg;

public static class JwtBearerConfiguration
{
    public static Action<JwtBearerOptions> Create(OauthOptions oauthOptions)
    {
        return jwtBearerOptions =>
        {
            jwtBearerOptions.Authority = oauthOptions.Authority;
            jwtBearerOptions.Audience = oauthOptions.Audience;
        };
    }
}