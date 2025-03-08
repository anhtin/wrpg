using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Wrpg.Shared.Database;

namespace Helpers;

public class AppDbContextFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().Build();

    public AppDbContext AppDbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var connectionString = $"{_dbContainer.GetConnectionString()};Include Error Detail=true";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString
            })
            .Build();
        AppDbContext = new AppDbContext(AppDbContext.Configure(configuration, new DbContextOptionsBuilder()).Options);
        await AppDbContext.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}