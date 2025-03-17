using System.Diagnostics.CodeAnalysis;

namespace Wrpg;

public static class AdventureName
{
    public const int MinLength = 1;
    public const string MinLengthErrorMessage = "Adventure name cannot be empty";

    public const int MaxLength = 50;
    public static readonly string MaxLengthErrorMessage = $"Adventure name cannot exceed {MaxLength} characters";

    public static string Generate() => "The Fool's Errand";

    public static bool IsValid(string name, [NotNullWhen(false)] out string? error)
    {
        name = Normalized(name);

        if (name.Length < MinLength)
        {
            error = MinLengthErrorMessage;
            return false;
        }

        if (name.Length > MaxLength)
        {
            error = MaxLengthErrorMessage;
            return false;
        }

        error = null;
        return true;
    }

    public static string Normalized(string name) => name.Trim();
}