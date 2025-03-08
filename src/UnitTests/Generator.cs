public static class Generator
{
    public static string RandomString() => Guid.NewGuid().ToString();

    public static int RandomInt() => Random.Shared.Next();
}