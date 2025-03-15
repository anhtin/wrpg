using Wrpg;

namespace Features.Character;

public class CharacterNameTest
{
    [Theory]
    [InlineData("nickname-with-hyphens")]
    [InlineData("nickname_with_underscore")]
    public void IsValid_returns_true_when_name_is_valid(string name)
    {
        var result = CharacterName.IsValid(name);
        Assert.True(result);
    }

    [Theory]
    [InlineData("Capital")]
    [InlineData("s p a c e")]
    [InlineData("$ymbols")]
    public void IsValid_returns_false_when_CharacterName_contains_invalid_characters(string name)
    {
        var result = CharacterName.IsValid(name);
        Assert.False(result);
    }

    [Theory]
    [InlineData("UPPER", "upper")]
    [InlineData("Capital_Letters", "capital_letters")]
    public void Normalize_returns_CharacterName_in_all_lowercase(string name, string expected)
    {
        var actual = CharacterName.Normalize(name);
        Assert.Equal(expected, actual);
    }
}