public static class Generator
{
    private static readonly Random Random = new();

    public static string RandomString(int? length = null, bool includeSpace = true)
    {
        const string alphabetIncludingSpace = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        const string alphabetExcludingSpace = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var alphabet = includeSpace ? alphabetIncludingSpace : alphabetExcludingSpace;
        return new string(Enumerable
            .Repeat(alphabet, length ?? 10)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
    }

    public static int RandomInt(int? minValue = null, int? maxValue = null) =>
        Random.Next(minValue ?? int.MinValue, maxValue ?? int.MaxValue);
}