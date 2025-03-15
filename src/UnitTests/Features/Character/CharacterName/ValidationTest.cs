namespace Features.Character.CharacterName;

public class ValidationTest
{
    [Theory]
    [InlineData("nickname-with-hyphens")]
    [InlineData("nickname_with_underscore")]
    public void IsValid_returns_true_when_name_is_valid(string name)
    {
        var result = Wrpg.CharacterName.IsValid(name);
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
        var result = Wrpg.CharacterName.IsValid(name);
        Assert.False(result);
    }
}