using System.Diagnostics.CodeAnalysis;

namespace DefaultNamespace;

public class ReflectionEqualityComparer<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var xValue = property.GetValue(x);
            var yValue = property.GetValue(x);
            if (!Equals(xValue, yValue)) return false;
        }

        return true;
    }

    public int GetHashCode([DisallowNull] T obj)
    {
        return obj.GetHashCode();
    }
}