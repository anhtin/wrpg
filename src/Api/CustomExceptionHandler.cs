using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Wrpg;

public static class CustomExceptionHandler
{
    public static RequestDelegate CreateDelegate(bool isDevelopment) => async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()!.Error;
        switch (exception)
        {
            case DbUpdateException { InnerException: PostgresException postgresException }
                when postgresException.Message.StartsWith("23505: duplicate key value violates unique constraint"):
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                break;
            }

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                if (isDevelopment) await context.Response.WriteAsync(exception.ToString());
                break;
        }
    };
}