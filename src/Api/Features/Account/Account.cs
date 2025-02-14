using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public sealed class Account
{
    public int Id { get; internal set; }
    public string IdentityProvider { get; internal set; } = null!;
    public string IdentityId { get; internal set; } = null!;
    public string Nickname { get; internal set; } = null!;

    public static Account CreateNew(string identityProvider, string identityId, string nickname) => new()
    {
        IdentityProvider = identityProvider,
        IdentityId = identityId,
        Nickname = nickname,
    };

    internal static void Configure(EntityTypeBuilder<Account> entity)
    {
        entity.HasKey(x => x.Id);
        entity.HasIndex(nameof(IdentityProvider), nameof(IdentityId)).IsUnique();
        entity.HasIndex(x => x.Nickname).IsUnique();

        entity.Property(x => x.Id).UseSerialColumn().ValueGeneratedOnAdd();
        entity.Property(x => x.IdentityProvider).IsRequired().HasMaxLength(50);
        entity.Property(x => x.IdentityId).IsRequired().HasMaxLength(50);
        entity.Property(x => x.Nickname).IsRequired().HasMaxLength(20);
    }
}