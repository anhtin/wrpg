using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Wrpg;

public static class OpenApiConfiguration
{
    public static Action<OpenApiOptions> Create(OauthOptions oauthOptions)
    {
        return openApiOptions =>
        {
            openApiOptions.AddDocumentTransformer((document, _, _) =>
            {
                var authorizationUrl = $"{oauthOptions.Authority}/authorize?audience={oauthOptions.Audience}";
                var tokenUrl = $"{oauthOptions.Authority}/oauth/token";
                AddImplicitOauthFlow(document, authorizationUrl, tokenUrl);
                return Task.CompletedTask;
            });

            openApiOptions.AddSchemaTransformer((schema, context, _) =>
            {
                if (schema.Annotations == null || !schema.Annotations.ContainsKey("x-schema-id"))
                    return Task.CompletedTask;

                var type = context.JsonTypeInfo.Type;
                if (!type.IsNested || type.DeclaringType is null)
                    return Task.CompletedTask;

                schema.Title = $"{type.DeclaringType.Name}.{type.Name}";
                schema.Annotations["x-schema-id"] = schema.Title;

                return Task.CompletedTask;
            });
        };
    }

    private static void AddImplicitOauthFlow(OpenApiDocument document, string authorizationUrl, string tokenUrl)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            BearerFormat = "JWT",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(authorizationUrl),
                    TokenUrl = new Uri(tokenUrl),
                    Scopes = new Dictionary<string, string>
                    {
                        ["read:character:all"] = "See any character",
                        ["write:character:all"] = "Modify any character",
                        ["read:character:own"] = "See own characters",
                        ["write:character:own"] = "Modify own characters",
                    }
                }
            },
            Reference = new() { Id = "oauth2", Type = ReferenceType.SecurityScheme, },
        };

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes[scheme.Reference.Id] = scheme;
        document.SecurityRequirements ??= [];
        document.SecurityRequirements.Add(new() { [scheme] = [] });
    }
}