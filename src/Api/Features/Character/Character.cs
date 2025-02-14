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

        entity.ComplexProperty(x => x.Stats, Stats.Configure);
    }
}

[Owned]
public class Stats
{
    public Attributes Attributes { get; internal set; } = null!;
    public Resources Resources { get; internal set; } = null!;

    public static Stats CreateNew()
    {
        var defaultAttributes = Attributes.CreateNew();
        return new Stats
        {
            Attributes = defaultAttributes,
            Resources = Resources.CreateFrom(defaultAttributes),
        };
    }

    internal static void Configure(ComplexPropertyBuilder<Stats> entity)
    {
        entity.IsRequired();
        entity.ComplexProperty(x => x.Attributes, Attributes.Configure);
        entity.ComplexProperty(x => x.Resources, Resources.Configure);
    }
}

[Owned]
public class Attributes
{
    public int Level { get; internal set; }
    public int Strength { get; internal set; }
    public int Dexterity { get; internal set; }
    public int Intelligence { get; internal set; }
    public int Constitution { get; internal set; }
    public int Spirit { get; internal set; }

    public static Attributes CreateNew() => new()
    {
        Level = 1,
        Strength = 10,
        Dexterity = 10,
        Intelligence = 10,
        Constitution = 10,
        Spirit = 10,
    };

    internal static void Configure(ComplexPropertyBuilder<Attributes> entity)
    {
        entity.IsRequired();
        entity.Property(x => x.Level).IsRequired();
        entity.Property(x => x.Strength).IsRequired();
        entity.Property(x => x.Dexterity).IsRequired();
        entity.Property(x => x.Intelligence).IsRequired();
        entity.Property(x => x.Constitution).IsRequired();
        entity.Property(x => x.Spirit).IsRequired();
    }
}

[Owned]
public class Resources
{
    public int Health { get; internal set; }
    public int Energy { get; internal set; }

    public static Resources CreateFrom(Attributes attributes)
    {
        const int constitutionFactorOnHealth = 1;
        const int spiritFactorOnEnergy = 1;
        return new Resources
        {
            Health = attributes.Level * constitutionFactorOnHealth * attributes.Constitution,
            Energy = attributes.Level * spiritFactorOnEnergy * attributes.Spirit,
        };
    }

    internal static void Configure(ComplexPropertyBuilder<Resources> entity)
    {
        entity.IsRequired();
        entity.Property(x => x.Health).IsRequired();
        entity.Property(x => x.Energy).IsRequired();
    }
}