﻿using Microsoft.AspNetCore.OpenApi;
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