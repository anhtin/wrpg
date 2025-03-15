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
    [InlineData("UPPER")]
    [InlineData("Capital")]
    public void IsValid_returns_false_when_name_contains_uppercase_letters(string name) => AssertInvalid(name);

    [Theory]
    [InlineData(" leading")]
    [InlineData("trailing ")]
    [InlineData("mid dle")]
    public void IsValid_returns_false_when_name_contains_space_character(string name) => AssertInvalid(name);

    [Theory]
    [InlineData("!")]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("$")]
    [InlineData("%")]
    [InlineData("^")]
    [InlineData("&")]
    [InlineData("*")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("+")]
    public void IsValid_returns_false_when_name_contains_special_symbol(string name) => AssertInvalid(name);

    private void AssertInvalid(string name)
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