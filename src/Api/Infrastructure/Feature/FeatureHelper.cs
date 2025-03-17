namespace Wrpg;

public static class FeatureHelper
{
    public static async Task<THttpResult> Execute<TData, THttpResult, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, FeatureResult<THttpResult, TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
        where THttpResult : IResult
    {
        var data = await loadData();
        var result = executeLogic(data);
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static async Task<THttpResult> Execute<THttpResult, TSideEffects>(
        Func<FeatureResult<THttpResult, TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
        where THttpResult : IResult
    {
        var result = executeLogic();
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static THttpResult Execute<THttpResult>(Func<FeatureResult<THttpResult>> executeLogic)
        where THttpResult : IResult
    {
        var result = executeLogic();
        return result.Http;
    }

    public static async Task<THttpResult> TryExecute<TData, THttpResult, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, FeatureResult<THttpResult, TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where THttpResult : IResult
    {
        try
        {
            var data = await loadData();
            var result = executeLogic(data);
            await executeSideEffects(result.SideEffects);
            return result.Http;
        }
        catch (Exception e)
        {
            var overridingResult = await exceptionHandler(e);
            if (overridingResult is not null) return overridingResult;
            throw;
        }
    }

    public static async Task<THttpResult> TryExecute<THttpResult, TSideEffects>(
        Func<FeatureResult<THttpResult, TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where THttpResult : IResult
    {
        try
        {
            var result = executeLogic();
            await executeSideEffects(result.SideEffects);
            return result.Http;
        }
        catch (Exception e)
        {
            var overridingResult = await exceptionHandler(e);
            if (overridingResult is not null) return overridingResult;
            throw;
        }
    }

    public static async Task<THttpResult> TryExecute<THttpResult>(
        Func<FeatureResult<THttpResult>> executeLogic,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where THttpResult : IResult
    {
        try
        {
            var result = executeLogic();
            return result.Http;
        }
        catch (Exception e)
        {
            var overridingResult = await exceptionHandler(e);
            if (overridingResult is not null) return overridingResult;
            throw;
        }
    }
}