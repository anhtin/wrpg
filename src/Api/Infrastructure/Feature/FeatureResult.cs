namespace Wrpg;

public sealed class FeatureResult<THttpResult, TSideEffects> where THttpResult : IResult
{
    public required THttpResult Http { get; init; }
    public TSideEffects SideEffects { get; init; } = default!;
}

public sealed class FeatureResult<THttpResult> where THttpResult : IResult
{
    public required THttpResult Http { get; init; }
}