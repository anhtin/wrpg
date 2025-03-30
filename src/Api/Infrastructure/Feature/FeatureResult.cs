using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Wrpg;

public sealed class FeatureResult<TSideEffects>
{
    public required IResult Http { get; init; }
    public TSideEffects SideEffects { get; init; } = default!;


    // @formatter:off
    public static implicit operator FeatureResult<TSideEffects>(Ok result) => new() { Http = result };
    public static implicit operator FeatureResult<TSideEffects>(BadRequest<ProblemDetails> result) => new() { Http = result };
    public static implicit operator FeatureResult<TSideEffects>(NotFound result) => new() { Http = result };
    // @formatter:on
}

public sealed class FeatureResult
{
    public required IResult Http { get; init; }

    public static implicit operator FeatureResult(Ok result) => new() { Http = result };
    public static implicit operator FeatureResult(BadRequest<ProblemDetails> result) => new() { Http = result };
    public static implicit operator FeatureResult(NotFound result) => new() { Http = result };
}