using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

const bool resolveLocalCredentials = true;
const bool resolvePipelineCredentials = false;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var appConfig = configuration.GetSection("Tools:CredentialsResolver");

if (resolveLocalCredentials)
{
    var oauthOptions = CreateOauthOptions(appConfig.GetRequiredSection("Local:IdentityProvider"));
    await OutputAccessToken("Local", "Player1", oauthOptions);
    await OutputAccessToken("Local", "Player2", oauthOptions);
    await OutputAccessToken("Local", "Admin", oauthOptions);
}

if (resolvePipelineCredentials)
{
    var oauthOptions = CreateOauthOptions(appConfig.GetRequiredSection("Pipeline:IdentityProvider"));
    await OutputAccessToken("Pipeline", "Player1", oauthOptions);
    await OutputAccessToken("Pipeline", "Player2", oauthOptions);
    await OutputAccessToken("Pipeline", "Admin", oauthOptions);
}

async Task OutputAccessToken(string environment, string user, OauthOptions oauthOptions)
{
    var config = appConfig.GetRequiredSection($"{environment}:Credentials:{user}");
    var credentials = CreateCredentials(config);
    var token = await GetAccessToken(oauthOptions, credentials);
    Console.WriteLine($"{user}: {token}");
    Console.WriteLine();
}

OauthOptions CreateOauthOptions(IConfiguration configuration) => new()
{
    Authority = configuration["Authority"]!,
    Audience = configuration["Audience"]!,
};

OAuthClientCredentials CreateCredentials(IConfiguration configuration) => new()
{
    ClientId = configuration["ClientId"]!,
    ClientSecret = configuration["ClientSecret"]!,
};

async Task<string> GetAccessToken(OauthOptions oauthOptions, OAuthClientCredentials credentials)
{
    using var httpClient = new HttpClient();
    var tokenUrl = $"{oauthOptions.Authority}/oauth/token";
    var response = await httpClient.PostAsJsonAsync(tokenUrl, new
    {
        client_id = credentials.ClientId,
        client_secret = credentials.ClientSecret,
        audience = oauthOptions.Audience,
        grant_type = "client_credentials",
    });
    response.EnsureSuccessStatusCode();

    var responseBody = await response.Content.ReadFromJsonAsync<JsonObject>();
    if (responseBody is null ||
        !responseBody.TryGetPropertyValue("access_token", out var tokenNode) ||
        tokenNode is null)
    {
        throw new ApplicationException("Unexpected token response body");
    }

    return tokenNode.GetValue<string>();
}

class OauthOptions
{
    public required string Authority { get; init; }
    public required string Audience { get; init; }
}

class OAuthClientCredentials
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}