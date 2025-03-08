using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wrpg;

namespace Helpers;

public static class WebApplicationFactoryExtensions
{
    public static JsonSerializerOptions GetJsonSerializerOptions(this WebApplicationFactory<Program> app) =>
        app.Services.GetRequiredService<IOptions<JsonOptions>>().Value.SerializerOptions;
}