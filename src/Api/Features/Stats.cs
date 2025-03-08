using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

[Owned]
public sealed class Stats
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

    internal static void Configure<TOwner>(OwnedNavigationBuilder<TOwner, Stats> builder) where TOwner : class
    {
        builder.OwnsOne(x => x.Attributes, Attributes.Configure);
        builder.OwnsOne(x => x.Resources, Resources.Configure);
    }
}

[Owned]
public sealed class Attributes
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

    internal static void Configure<TOwner>(OwnedNavigationBuilder<TOwner, Attributes> builder) where TOwner : class
    {
        builder.Property(x => x.Level).IsRequired();
        builder.Property(x => x.Strength).IsRequired();
        builder.Property(x => x.Dexterity).IsRequired();
        builder.Property(x => x.Intelligence).IsRequired();
        builder.Property(x => x.Constitution).IsRequired();
        builder.Property(x => x.Spirit).IsRequired();
    }
}

[Owned]
public sealed class Resources
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

    internal static void Configure<TOwner>(OwnedNavigationBuilder<TOwner, Resources> builder) where TOwner : class
    {
        builder.Property(x => x.Health).IsRequired();
        builder.Property(x => x.Energy).IsRequired();
    }
}
