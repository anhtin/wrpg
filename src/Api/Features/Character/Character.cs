using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public sealed class Character : IEntityTypeConfiguration<Character>
{
    public int Id { get; internal set; }
    public string UserId { get; internal set; } = null!;
    public string Name { get; internal set; } = null!;
    public Stats Stats { get; internal set; } = null!;

    public static Character CreateNew(string name, string userId) => new()
    {
        UserId = userId,
        Name = name,
        Stats = Stats.CreateNew(),
    };

    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Id).UseSerialColumn().ValueGeneratedOnAdd();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(20);

        builder.OwnsOne(x => x.Stats, Stats.Configure);
    }
}