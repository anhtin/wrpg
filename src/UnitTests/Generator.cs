public static class Generator
{
    private static readonly Random Random = new();

    public static string RandomString(int? length = null)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        return new string(Enumerable
            .Repeat(chars, length ?? 10)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
    }

    public static int RandomInt(int? minValue = null, int? maxValue = null) =>
        Random.Next(minValue ?? int.MinValue, maxValue ?? int.MaxValue);
}