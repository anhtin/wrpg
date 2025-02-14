namespace Wrpg.UnitTests;

public static class AccountGenerator
{
    public static Account Create(
        int id = 0,
        string? identityProvider = null,
        string? identityId = null,
        string? nickname = null) => new()
    {
        Id = id,
        IdentityProvider = identityProvider ?? Generator.RandomString(),
        IdentityId = identityId ?? Generator.RandomString(),
        Nickname = nickname ?? Generator.RandomString(),
    };
}