namespace Helpers;

public class SmokeTestContext(Sut sut) : IClassFixture<Sut>
{
    public Sut Sut { get; } = sut;

    public HttpClient HttpClient { get; } = sut.CreateClient();
}