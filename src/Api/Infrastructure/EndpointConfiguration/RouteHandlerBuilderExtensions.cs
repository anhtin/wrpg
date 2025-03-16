namespace Wrpg;

public static class RouteHandlerBuilderExtensions
{
    public static TBuilder RequirePermissionAny<TBuilder>(this TBuilder builder, params string[] permissions)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization(policy =>
            policy.RequireAssertion(context => context.User.HasClaim(claim =>
                claim.Type is "scope" &&
                claim.Value.Split(' ').Intersect(permissions).Any())));
    }

    public static TBuilder RequirePermissionAll<TBuilder>(this TBuilder builder, params string[] permissions)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization(policy =>
            policy.RequireAssertion(context => context.User.HasClaim(claim =>
                claim.Type is "scope" &&
                !permissions.Except(claim.Value.Split(' ')).Any())));
    }
}