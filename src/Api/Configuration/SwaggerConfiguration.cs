using Swashbuckle.AspNetCore.SwaggerUI;

namespace Wrpg;

public static class SwaggerConfiguration
{
    public static Action<SwaggerUIOptions> Create(SwaggerOptions options)
    {
        return swaggerUiOptions =>
        {
            swaggerUiOptions.RoutePrefix = "docs";
            swaggerUiOptions.SwaggerEndpoint("/openapi/v1.json", "API Spec");
            swaggerUiOptions.OAuthAppName("Swagger Client");
            swaggerUiOptions.OAuthClientId(options.Credentials.ClientId);
            swaggerUiOptions.OAuthClientSecret(options.Credentials.ClientSecret);
        };
    }
}