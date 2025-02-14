using Wrpg.IntegrationTests.Helpers;
using Wrpg.Shared.Database;

namespace Wrpg.IntegrationTests;

public class AppDbContextTest(AppDbContextFixture context) : IClassFixture<AppDbContextFixture>
{
    private AppDbContext Sut { get; } = context.AppDbContext;

    [Fact]
    public async Task Can_connect_to_database()
    {
        await Sut.Database.CanConnectAsync();
    }

    [Fact]
    public async Task Can_load_Account_along_with_related_Characters()
    {
        var account = new Account
        {
            Id = 3,
            IdentityProvider = "IP",
            IdentityId = "ID",
            Nickname = "dylan",
        };
        var character1 = new Character
        {
            AccountId = account.Id,
            Name = "big-dylan",
            Stats = Stats.CreateNew(),
        };
        var character2 = new Character
        {
            AccountId = account.Id,
            Name = "amazing-dylan",
            Stats = Stats.CreateNew(),
        };
        var character3 = new Character
        {
            AccountId = 4,
            Name = "not-dylan",
            Stats = Stats.CreateNew(),
        };
        var character4 = new Character
        {
            AccountId = account.Id,
            Name = "lazy-dylan",
            Stats = Stats.CreateNew(),
        };

        Sut.Accounts.Add(account);
        Sut.Characters.Add(character1);
        Sut.Characters.Add(character2);
        Sut.Characters.Add(character3);
        Sut.Characters.Add(character4);
        await Sut.SaveChangesAsync();

        var data = await DeleteAccount.LoadData(
            new DeleteAccount.Command
            {
                Nickname = account.Nickname,
            },
            Sut);

        Assert.NotNull(data);
        Assert.Multiple(
            () => Assert.Equal(account, data.Account),
            () => Assert.Collection(
                data.Characters,
                x => Assert.Equal(character1, x),
                x => Assert.Equal(character2, x),
                x => Assert.Equal(character4, x)));
    }
}