namespace Features.Character.CharacterName;

public class NormalizationTest
{
    [Theory]
    [InlineData("UPPER", "upper")]
    [InlineData("Capital_Letters", "capital_letters")]
    public void Normalize_returns_CharacterName_in_all_lowercase(string name, string expected)
    {
        var actual = Wrpg.CharacterName.Normalize(name);
        Assert.Equal(expected, actual);
    }
}