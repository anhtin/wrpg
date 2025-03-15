using Wrpg;

var builder = WebApplication.CreateBuilder(args);

var oauthOptions = OauthOptions.CreateFrom(builder.Configuration);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerConfiguration.Create(oauthOptions));
builder.Services.AddAuthorization(AuthorizationConfiguration.Configure);
builder.Services.AddOpenApi(OpenApiConfiguration.Create(oauthOptions));
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")!;
    AppDbContext.ConfigurePostgreSql(connectionString, optionsBuilder);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var swaggerUiOptions = SwaggerOptions.CreateFrom(builder.Configuration);
    app.UseSwaggerUI(SwaggerConfiguration.Create(swaggerUiOptions));
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(CustomExceptionHandler.CreateOptions(app.Environment.IsDevelopment()));

app.MapFeatureEndpointsFromAssembly(typeof(Program).Assembly);

app.Run();


namespace Wrpg
{
    public partial class Program;
}