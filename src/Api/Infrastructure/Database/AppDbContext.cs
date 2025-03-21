﻿using Microsoft.EntityFrameworkCore;

namespace Wrpg;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Adventure> Adventures { get; set; }
    public DbSet<Character> Characters { get; set; }

    public static DbContextOptionsBuilder ConfigurePostgreSql(
        string connectionString,
        DbContextOptionsBuilder? builder = null)
    {
        builder ??= new DbContextOptionsBuilder();
        return builder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}