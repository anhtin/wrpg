using Wrpg;
using Wrpg.Shared.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
    AppDbContext.Configure(builder.Configuration, optionsBuilder));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseReDoc(options =>
    // {
    //     options.RoutePrefix = "docs";
    //     options.SpecUrl = "/openapi/v1.json";
    // });
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "docs";
        options.SwaggerEndpoint("/openapi/v1.json", "API Spec");
    });
}

app.UseHttpsRedirection();

app.MapAccountEndpoints();
app.MapCharacterEndpoints();


app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = CustomExceptionHandler.CreateDelegate(app.Environment.IsDevelopment()),
});

app.Run();

namespace Wrpg
{
    public partial class Program;
}