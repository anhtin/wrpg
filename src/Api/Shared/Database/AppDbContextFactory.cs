using Microsoft.EntityFrameworkCore.Design;

namespace Wrpg.Shared.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        return new AppDbContext(AppDbContext.Configure(configuration).Options);
    }
}