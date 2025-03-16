using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using Xunit.Sdk;

namespace Helpers;

public static class HttpAssert
{
    public static async Task Status(HttpStatusCode expectedStatusCode, HttpResponseMessage response)
    {
        if (response.StatusCode == expectedStatusCode)
            return;

        var errorMessage = await CreateErrorMessage(
            "Unexpected status code",
            $"{(int)expectedStatusCode} {expectedStatusCode}",
            $"{(int)response.StatusCode} {response.StatusCode}",
            response);
        throw new HttpAssertException(errorMessage);
    }

    public static async Task Status(Expression<Func<HttpStatusCode, bool>> predicate, HttpResponseMessage response)
    {
        if (predicate.Compile().Invoke(response.StatusCode))
            return;

        var errorMessage = await CreateErrorMessage(
            "Unexpected status code",
            $"{predicate}",
            $"{(int)response.StatusCode} {response.StatusCode}",
            response);
        throw new HttpAssertException(errorMessage);
    }

    public static Task Header(string name, string expectedValue, HttpResponseMessage response) =>
        Header(name, [expectedValue], response);

    public static async Task Header(string name, string[] expectedValues, HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues(name, out var actualValues) && expectedValues.Length > 0)
        {
            throw new HttpAssertException(await CreateErrorMessage(
                $"Unexpected value on header '{name}'",
                $"{string.Join(',', expectedValues)}",
                "",
                response));
        }

        actualValues = actualValues?.ToArray() ?? [];
        if (!actualValues.SequenceEqual(expectedValues))
        {
            throw new HttpAssertException(await CreateErrorMessage(
                $"Unexpected value on header '{name}'",
                $"{string.Join(',', expectedValues)}",
                $"{string.Join(',', actualValues)}",
                response));
        }
    }

    public static async Task Body(
        object expectedBody,
        HttpResponseMessage response,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var body = await response.Content.ReadAsStringAsync();
        var expected = JsonSerializer.Serialize(expectedBody, jsonSerializerOptions);
        try
        {
            Assert.Equivalent(expected, body);
        }
        catch (EquivalentException)
        {
            throw new HttpAssertException(
                await CreateErrorMessage("Unexpected response body", expected, body, response));
        }
    }

    public static async Task Body<T>(
        Expression<Predicate<T?>> bodyInspector,
        HttpResponseMessage response,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        jsonSerializerOptions ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var bodyString = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<T>(bodyString, jsonSerializerOptions);
        var compiledBodyInspector = bodyInspector.Compile();
        try
        {
            compiledBodyInspector.Invoke(body);
        }
        catch (EquivalentException)
        {
            throw new HttpAssertException(
                await CreateErrorMessage("Unexpected response body", bodyInspector.ToString(), bodyString, response));
        }
    }

    private static async Task<string> CreateErrorMessage(
        string message,
        string expected,
        string actual,
        HttpResponseMessage response) =>
        $"""
         {message}
         Expected: {expected}
         Actual:   {actual}

         Request:
         {response.RequestMessage}

         Response:
         {response}
         {await response.Content.ReadAsStringAsync()}
         """;
}

public class HttpAssertException(string message) : Exception(message);