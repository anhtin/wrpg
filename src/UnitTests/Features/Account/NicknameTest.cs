namespace Wrpg.UnitTests;

public class NicknameTest
{
    [Theory]
    [InlineData("nickname-with-hyphens")]
    [InlineData("nickname_with_underscore")]
    public void IsValid_returns_true_when_Nickname_is_valid(string nickname)
    {
        var result = Nickname.IsValid(nickname);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Capital")]
    [InlineData("s p a c e")]
    [InlineData("$ymbols")]
    public void IsValid_returns_false_when_Nickname_contains_invalid_characters(string nickname)
    {
        var result = Nickname.IsValid(nickname);
        Assert.False(result);
    }

    [Theory]
    [InlineData("UPPER", "upper")]
    [InlineData("Capital_Letters", "capital_letters")]
    public void Normalize_returns_Nickname_in_all_lowercase(string nickname, string expected)
    {
        var actual = Nickname.Normalize(nickname);
        Assert.Equal(expected, actual);
    }
}