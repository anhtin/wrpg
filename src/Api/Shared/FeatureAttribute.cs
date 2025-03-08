using System.Reflection;

namespace Wrpg.Shared;

[AttributeUsage(AttributeTargets.Class)]
public class FeatureAttribute : Attribute;

public static class EndpointRouteBuilderExtensions
{
    public static TEndpointRouteBuilder MapFeatureEndpointsFromAssembly<TEndpointRouteBuilder>(
        this TEndpointRouteBuilder builder,
        Assembly assembly)
    {
        var features = assembly.GetTypes().Where(x => x.GetCustomAttribute<FeatureAttribute>() is not null);
        foreach (var feature in features)
        {
            var configureMethod = feature.GetMethod(
                "ConfigureEndpoints",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                [typeof(IEndpointRouteBuilder)]);

            configureMethod?.Invoke(null, [builder]);
        }

        return builder;
    }
}