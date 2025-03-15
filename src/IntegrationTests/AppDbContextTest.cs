using Helpers;
using Microsoft.EntityFrameworkCore;
using Wrpg;

public class AppDbContextTest(AppDbContextFixture context) : IClassFixture<AppDbContextFixture>
{
    private AppDbContext Sut { get; } = context.AppDbContext;

    [Fact]
    public async Task Can_connect_to_database()
    {
        await Sut.Database.CanConnectAsync();
    }

    [Fact]
    public async Task Can_retrieve_all_character_related_to_user()
    {
        // Arrange
        const string userId = "some-user";
        var character1 = new Character
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "big-dylan",
            Stats = Stats.CreateNew(),
        };
        var character2 = new Character
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "amazing-dylan",
            Stats = Stats.CreateNew(),
        };
        var character3 = new Character
        {
            Id = Guid.NewGuid(),
            UserId = "some-other-user",
            Name = "not-dylan",
            Stats = Stats.CreateNew(),
        };
        var character4 = new Character
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "lazy-dylan",
            Stats = Stats.CreateNew(),
        };

        Sut.Characters.Add(character1);
        Sut.Characters.Add(character2);
        Sut.Characters.Add(character3);
        Sut.Characters.Add(character4);
        await Sut.SaveChangesAsync();

        // Act
        var characters = await Sut.Characters.Where(x => x.UserId == userId).ToArrayAsync();

        // Assert
        Assert.Collection(
            characters,
            x => Assert.Equal(character1, x),
            x => Assert.Equal(character2, x),
            x => Assert.Equal(character4, x));
    }
}