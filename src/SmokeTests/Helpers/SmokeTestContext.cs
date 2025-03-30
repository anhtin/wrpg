using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers;

public class SmokeTestContext : IClassFixture<Sut>
{
    protected Sut Sut { get; }
    protected HttpClient UnauthorizedClient { get; }
    protected HttpClient AdminClient { get; }
    protected HttpClient Player1Client { get; }
    protected HttpClient Player2Client { get; }

    protected HttpClient PlayerClient => Player1Client;

    public SmokeTestContext(Sut sut)
    {
        Sut = sut;
        UnauthorizedClient = sut.CreateClient();

        var configuration = (IConfigurationRoot)Sut.Services.GetRequiredService<IConfiguration>();
        var smokeTestOauthOptions = SmokeTestAuthOptions.CreateFrom(configuration);
        AdminClient = CreateAuthorizedClient(smokeTestOauthOptions.AccessTokens.Admin);
        Player1Client = CreateAuthorizedClient(smokeTestOauthOptions.AccessTokens.Player1);
        Player2Client = CreateAuthorizedClient(smokeTestOauthOptions.AccessTokens.Player2);
    }

    private HttpClient CreateAuthorizedClient(string accessToken)
    {
        var sutClient = Sut.CreateClient();
        sutClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return sutClient;
    }
}