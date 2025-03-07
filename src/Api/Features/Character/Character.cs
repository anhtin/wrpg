using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public sealed class Character
{
    public int Id { get; internal set; }
    public int AccountId { get; internal set; }
    public string Name { get; internal set; } = null!;
    public Stats Stats { get; internal set; } = null!;

    public static Character CreateNew(string name, int accountId) => new()
    {
        AccountId = accountId,
        Name = name,
        Stats = Stats.CreateNew(),
    };

    internal static void Configure(EntityTypeBuilder<Character> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasIndex(x => x.Name).IsUnique();

        entity.Property(x => x.Id).UseSerialColumn().ValueGeneratedOnAdd();
        entity.Property(x => x.AccountId);
        entity.Property(x => x.Name).IsRequired().HasMaxLength(20);

        entity.OwnsOne(x => x.Stats, Stats.Configure);
    }
}