using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public class Adventure : IEntityTypeConfiguration<Adventure>
{
    public Guid Id { get; internal set; }
    public int InternalId { get; internal set; }
    public string UserId { get; internal set; }
    public Guid CharacterId { get; internal set; }
    public string Name { get; internal set; }
    public AdventureStatus Status { get; internal set; }
    public string LocationName { get; internal set; }

    public static Adventure CreateNew(Guid id, string userId, Guid characterId, string name) => new()
    {
        Id = id,
        UserId = userId,
        CharacterId = characterId,
        Name = name,
        Status = AdventureStatus.Ongoing,
        LocationName = Wrpg.LocationName.Start,
    };

    public void Configure(EntityTypeBuilder<Adventure> builder)
    {
        builder.HasKey(x => x.InternalId);
        builder.HasIndex(x => x.Id).IsUnique();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CharacterId);

        builder.Property(x => x.UserId).HasMaxLength(Wrpg.UserId.MaxLength);
        builder.Property(x => x.CharacterId);
        builder.Property(x => x.Name).HasMaxLength(AdventureName.MaxLength);
        builder.Property(x => x.LocationName);
    }
}

public enum AdventureStatus
{
    Ongoing,
    Ended,
}