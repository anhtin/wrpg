using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wrpg;

public sealed class Account : IEntityTypeConfiguration<Account>
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

    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(nameof(IdentityProvider), nameof(IdentityId)).IsUnique();
        builder.HasIndex(x => x.Nickname).IsUnique();

        builder.Property(x => x.Id).UseSerialColumn().ValueGeneratedOnAdd();
        builder.Property(x => x.IdentityProvider).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IdentityId).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Nickname).IsRequired().HasMaxLength(20);
    }
}