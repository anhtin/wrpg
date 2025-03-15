using Microsoft.EntityFrameworkCore.Design;

namespace Wrpg;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString();
        var dbContextOptions = AppDbContext.ConfigurePostgreSql(connectionString).Options;
        return new AppDbContext(dbContextOptions);
    }

    private static string ResolveConnectionString()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json");
        if (environment == "Development")
            configurationBuilder.AddJsonFile("appsettings.Development.json");

        var configuration = configurationBuilder.Build();
        return configuration.GetConnectionString("Default")!;
    }
}