namespace Wrpg;

public static class FeatureHelper
{
    public static async Task<THttpResult> Execute<TData, TResult, THttpResult, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, TResult> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
        where TResult : FeatureResult<THttpResult, TSideEffects>
    {
        var data = await loadData();
        var result = executeLogic(data);
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static async Task<THttpResult> Execute<TResult, THttpResult, TSideEffects>(
        Func<TResult> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
        where TResult : FeatureResult<THttpResult, TSideEffects>
    {
        var result = executeLogic();
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static THttpResult Execute<TResult, THttpResult>(Func<TResult> executeLogic)
        where TResult : FeatureResult<THttpResult>
    {
        var result = executeLogic();
        return result.Http;
    }

    public static async Task<THttpResult> TryExecute<TData, TResult, THttpResult, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, TResult> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where TResult : FeatureResult<THttpResult, TSideEffects>
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

    public static async Task<THttpResult> TryExecute<TResult, THttpResult, TSideEffects>(
        Func<TResult> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where TResult : FeatureResult<THttpResult, TSideEffects>
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

    public static async Task<THttpResult> TryExecute<TResult, THttpResult>(
        Func<TResult> executeLogic,
        Func<Exception, Task<THttpResult?>> exceptionHandler)
        where TResult : FeatureResult<THttpResult>
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