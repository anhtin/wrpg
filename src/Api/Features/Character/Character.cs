using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public sealed class Character : IEntityTypeConfiguration<Character>
{
    public const int MaxNameLength = 20;

    public Guid Id { get; internal set; }
    public int InternalId { get; internal set; }
    public string UserId { get; internal set; } = null!;
    public string Name { get; internal set; } = null!;
    public Stats Stats { get; internal set; } = null!;

    public static Character CreateNew(Guid id, string name, string userId) => new()
    {
        Id = id,
        UserId = userId,
        Name = name,
        Stats = Stats.CreateNew(),
    };

    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(x => x.InternalId);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.InternalId).UseSerialColumn().ValueGeneratedOnAdd();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(MaxNameLength);

        builder.OwnsOne(x => x.Stats, Stats.Configure);
    }
}