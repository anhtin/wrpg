using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wrpg;

namespace Helpers;

public class SmokeTestContext(Sut sut) : IClassFixture<Sut>, IAsyncLifetime
{
    protected Sut Sut { get; } = sut;
    protected HttpClient UnauthorizedClient { get; } = sut.CreateClient();
    protected HttpClient AdminClient { get; private set; } = null!;
    protected HttpClient Player1Client { get; private set; } = null!;
    protected HttpClient Player2Client { get; private set; } = null!;

    protected HttpClient PlayerClient => Player1Client;

    public async Task InitializeAsync()
    {
        var configuration = (IConfigurationRoot)Sut.Services.GetRequiredService<IConfiguration>();
        var oauthOptions = OauthOptions.CreateFrom(configuration);
        var smokeTestOauthOptions = SmokeTestOauthOptions.CreateFrom(configuration);
        AdminClient = await CreateAuthorizedClient(oauthOptions, smokeTestOauthOptions.Credentials.Admin);
        Player1Client = await CreateAuthorizedClient(oauthOptions, smokeTestOauthOptions.Credentials.Player1);
        Player2Client = await CreateAuthorizedClient(oauthOptions, smokeTestOauthOptions.Credentials.Player2);
    }

    public Task DisposeAsync()
    {
        UnauthorizedClient.Dispose();
        AdminClient.Dispose();
        Player1Client.Dispose();
        Player2Client.Dispose();
        return Task.CompletedTask;
    }

    private async Task<HttpClient> CreateAuthorizedClient(OauthOptions oauthOptions, OAuthClientCredentials credentials)
    {
        var accessToken = await GetAccessToken(oauthOptions, credentials);
        var sutClient = Sut.CreateClient();
        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return sutClient;
    }

    private async Task<string> GetAccessToken(OauthOptions oauthOptions, OAuthClientCredentials credentials)
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
}