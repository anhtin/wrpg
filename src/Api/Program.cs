using Wrpg;
using Wrpg.Shared.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
    AppDbContext.Configure(builder.Configuration, optionsBuilder));


var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler(CustomExceptionHandler.CreateOptions(app.Environment.IsDevelopment()));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "docs";
        options.SwaggerEndpoint("/openapi/v1.json", "API Spec");
    });
}


app.MapAccountEndpoints();
app.MapCharacterEndpoints();


app.Run();


namespace Wrpg
{
    public partial class Program;
}