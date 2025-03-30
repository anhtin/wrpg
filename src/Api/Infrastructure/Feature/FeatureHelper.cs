namespace Wrpg;

public static class FeatureHelper
{
    public static async Task<IResult> Execute<TData, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, FeatureResult<TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
    {
        var data = await loadData();
        var result = executeLogic(data);
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static async Task<IResult> Execute<TSideEffects>(
        Func<FeatureResult<TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects)
    {
        var result = executeLogic();
        await executeSideEffects(result.SideEffects);
        return result.Http;
    }

    public static IResult Execute(Func<FeatureResult> executeLogic)
    {
        var result = executeLogic();
        return result.Http;
    }

    public static async Task<IResult> TryExecute<TData, TSideEffects>(
        Func<Task<TData>> loadData,
        Func<TData, FeatureResult<TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<IResult?>> exceptionHandler)
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

    public static async Task<IResult> TryExecute<TSideEffects>(
        Func<FeatureResult<TSideEffects>> executeLogic,
        Func<TSideEffects, Task> executeSideEffects,
        Func<Exception, Task<IResult?>> exceptionHandler)
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

    public static async Task<IResult> TryExecute(
        Func<FeatureResult> executeLogic,
        Func<Exception, Task<IResult?>> exceptionHandler)
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