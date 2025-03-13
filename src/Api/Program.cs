using Wrpg;
using Wrpg.Shared;
using Wrpg.Shared.Database;

var builder = WebApplication.CreateBuilder(args);

var oauthOptions = OauthOptions.CreateFrom(builder.Configuration);
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerConfiguration.Create(oauthOptions));
builder.Services.AddAuthorization(AuthorizationConfiguration.Configure);
builder.Services.AddOpenApi(OpenApiConfiguration.Create(oauthOptions));
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
    AppDbContext.Configure(builder.Configuration, optionsBuilder));


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