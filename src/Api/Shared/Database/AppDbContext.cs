using Microsoft.EntityFrameworkCore;

namespace Wrpg.Shared.Database;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Character> Characters { get; set; }

    public static DbContextOptionsBuilder Configure(
        IConfiguration configuration,
        DbContextOptionsBuilder? builder = null)
    {
        builder ??= new DbContextOptionsBuilder();
        return builder.UseNpgsql(configuration.GetConnectionString("Default"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .AddAccountEntity()
            .AddCharacterEntity();
    }
}