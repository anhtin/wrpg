using System.Text.RegularExpressions;

namespace Wrpg;

public static partial class Nickname
{
    public const string Pattern = @"^[a-z0-9_\-]+$";

    [GeneratedRegex(Pattern)]
    private static partial Regex NicknameRegex();

    public static bool IsValid(string nickname) => NicknameRegex().IsMatch(nickname);

    public static string Normalize(string nickname) => nickname.ToLower();
}