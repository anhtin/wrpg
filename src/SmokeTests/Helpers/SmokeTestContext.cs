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
    protected HttpClient FullyAuthorizedClient { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        FullyAuthorizedClient = await CreateAuthorizedClient();
    }

    public Task DisposeAsync()
    {
        UnauthorizedClient.Dispose();
        FullyAuthorizedClient.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<HttpClient> CreateAuthorizedClient()
    {
        var sutClient = Sut.CreateClient();
        var accessToken = await GetAccessToken();
        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return sutClient;
    }

    private async Task<string> GetAccessToken()
    {
        var configuration = (IConfigurationRoot)Sut.Services.GetRequiredService<IConfiguration>();
        var oauthOptions = OauthOptions.CreateFrom(configuration);
        var smokeTestOauthOptions = SmokeTestOauthOptions.CreateFrom(configuration);

        using var httpClient = new HttpClient();
        var tokenUrl = $"{oauthOptions.Authority}/oauth/token";
        var response = await httpClient.PostAsJsonAsync(tokenUrl, new
        {
            client_id = smokeTestOauthOptions.ClientId,
            client_secret = smokeTestOauthOptions.ClientSecret,
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