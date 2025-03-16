namespace Wrpg;

public sealed class FeatureResult<THttpResult, TSideEffects>
{
    public required THttpResult Http { get; init; }
    public TSideEffects SideEffects { get; init; } = default!;
}

public abstract class FeatureResult<THttpResult>
{
    public required THttpResult Http { get; init; }
}
