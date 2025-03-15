using Testcontainers.PostgreSql;
using Wrpg;

namespace Helpers;

public class AppDbContextFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().Build();

    public AppDbContext AppDbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var connectionString = $"{_dbContainer.GetConnectionString()};Include Error Detail=true";
        var dbContextOptions = AppDbContext.ConfigurePostgreSql(connectionString).Options;
        AppDbContext = new AppDbContext(dbContextOptions);
        await AppDbContext.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}