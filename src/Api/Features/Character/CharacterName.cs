using System.Text.RegularExpressions;

namespace Wrpg;

public static partial class CharacterName
{
    public const string Pattern = @"^[a-z0-9_\-]+$";

    [GeneratedRegex(Pattern)]
    private static partial Regex CharacterNameRegex();

    public static bool IsValid(string nickname) => CharacterNameRegex().IsMatch(nickname);

    public static string Normalize(string nickname) => nickname.ToLower();
}