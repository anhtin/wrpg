using Xunit.Sdk;

public static class CustomAssert
{
    public static async Task Test(string description, Func<Task> testCode)
    {
        try
        {
            await testCode();
        }
        catch (Exception e)
        {
            throw new XunitException(description, e);
        }
    }
}