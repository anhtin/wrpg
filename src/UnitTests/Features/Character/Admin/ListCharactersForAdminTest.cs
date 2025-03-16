using DefaultNamespace;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wrpg;

namespace Features.Character.Admin;

public class ListCharactersForAdminTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Fails_when_page_number_is_invalid(int pageNumber)
    {
        // Arrange
        var dbContext = AppDbContextGenerator.Create();

        // Act
        var response = await ListCharactersForAdmin.Execute(pageNumber, null, dbContext);

        // Assert
        var result = Assert.IsType<BadRequest<ProblemDetails>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(ListCharactersForAdmin.InvalidPageNumberMessage, result.Value.Detail);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Fails_when_page_size_is_invalid(int pageSize)
    {
        // Arrange
        var dbContext = AppDbContextGenerator.Create();

        // Act
        var response = await ListCharactersForAdmin.Execute(null, pageSize, dbContext);

        // Assert
        var result = Assert.IsType<BadRequest<ProblemDetails>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(ListCharactersForAdmin.InvalidPageSizeMessage, result.Value.Detail);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(2, 1, 1, 1, 0)]
    [InlineData(1, 1, 2, 2, 1)]
    [InlineData(2, 1, 2, 2, 1)]
    [InlineData(3, 1, 2, 2, 0)]
    [InlineData(1, 2, 1, 1, 1)]
    [InlineData(2, 2, 1, 1, 0)]
    [InlineData(1, 2, 2, 1, 2)]
    [InlineData(2, 2, 2, 1, 0)]
    [InlineData(1, 2, 3, 2, 2)]
    [InlineData(2, 2, 3, 2, 1)]
    [InlineData(3, 2, 3, 2, 0)]
    public async Task Success_returns_correct_page_information(
        int? pageNumber,
        int? pageSize,
        int totalItemCount,
        int expectedTotalPageCount,
        int expectedItemCount)
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var characters = Enumerable.Range(0, totalItemCount).Select(_ => CharacterGenerator.Create(userId: userId));
        var dbContext = AppDbContextGenerator.Create(entities: characters.ToArray<object>());
        var user = ClaimsPrincipalGenerator.Create(userId: userId);

        // Act
        var response = await ListCharactersForPlayer.Execute(pageNumber, pageSize, user, dbContext);

        // Assert
        var result = Assert.IsType<Ok<Page<ListCharactersForPlayer.Character>>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Multiple(
            () => Assert.Equal(pageNumber, result.Value.PageNumber),
            () => Assert.Equal(pageSize, result.Value.PageSize),
            () => Assert.Equal(expectedTotalPageCount, result.Value.TotalPageCount),
            () => Assert.Equal(totalItemCount, result.Value.TotalItemCount),
            () => Assert.Equal(expectedItemCount, result.Value.Items.Count()));
    }

    [Fact]
    public async Task Success_returns_all_characters()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var character1 = CharacterGenerator.Create(userId: userId);
        var character2 = CharacterGenerator.Create(userId: Guid.NewGuid().ToString());
        var character3 = CharacterGenerator.Create(userId: userId);
        var dbContext = AppDbContextGenerator.Create(entities: [character1, character2, character3]);

        // Act
        var response = await ListCharactersForAdmin.Execute(null, null, dbContext);

        // Assert
        var result = Assert.IsType<Ok<Page<ListCharactersForAdmin.Character>>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Multiple(
            () => Assert.Contains(character1.Id, result.Value.Items.Select(x => x.Id)),
            () => Assert.Contains(character2.Id, result.Value.Items.Select(x => x.Id)),
            () => Assert.Contains(character3.Id, result.Value.Items.Select(x => x.Id)));
    }

    [Fact]
    public async Task Success_returns_characters_ordered_by_creation_time()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var character1 = CharacterGenerator.Create(internalId: 2, userId: userId);
        var character2 = CharacterGenerator.Create(internalId: 1, userId: userId);
        var dbContext = AppDbContextGenerator.Create(entities: [character1, character2]);

        // Act
        var response = await ListCharactersForAdmin.Execute(null, null, dbContext);

        // Assert
        var result = Assert.IsType<Ok<Page<ListCharactersForAdmin.Character>>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Multiple(
            () => Assert.Equal(1, result.Value.TotalPageCount),
            () => Assert.Equal(2, result.Value.TotalItemCount),
            () => Assert.Collection(
                result.Value.Items,
                item => Assert.Equal(character2.Id, item.Id),
                item => Assert.Equal(character1.Id, item.Id)));
    }

    [Fact]
    public async Task Success_returns_characters_with_correct_information()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var character1 = CharacterGenerator.Create(
            userId: userId,
            id: Guid.NewGuid(),
            name: Generator.RandomString(),
            stats: CreateRandomStats());
        var character2 = CharacterGenerator.Create(
            userId: userId,
            id: Guid.NewGuid(),
            name: Generator.RandomString(),
            stats: CreateRandomStats());
        var dbContext = AppDbContextGenerator.Create(entities: [character1, character2]);

        // Act
        var response = await ListCharactersForAdmin.Execute(null, null, dbContext);

        // Assert
        var equalityComparer = new ReflectionEqualityComparer<ListCharactersForAdmin.Character>();
        var result = Assert.IsType<Ok<Page<ListCharactersForAdmin.Character>>>(response.Result);
        Assert.NotNull(result.Value);
        Assert.Multiple(
            () => Assert.Equal(1, result.Value.TotalPageCount),
            () => Assert.Equal(2, result.Value.TotalItemCount),
            () =>
            {
                var expected = Map(character1);
                Assert.Contains(expected, result.Value.Items, equalityComparer);
            },
            () =>
            {
                var expected = Map(character2);
                Assert.Contains(expected, result.Value.Items, equalityComparer);
            });
    }

    private static Stats CreateRandomStats() => new()
    {
        Attributes = new()
        {
            Level = Generator.RandomInt(minValue: 0),
            Strength = Generator.RandomInt(minValue: 0),
            Constitution = Generator.RandomInt(minValue: 0),
            Dexterity = Generator.RandomInt(minValue: 0),
            Intelligence = Generator.RandomInt(minValue: 0),
            Spirit = Generator.RandomInt(minValue: 0),
        },
        Resources = new()
        {
            Health = Generator.RandomInt(minValue: 0),
            Energy = Generator.RandomInt(minValue: 0),
        },
    };

    private static ListCharactersForAdmin.Character Map(Wrpg.Character character) => new()
    {
        Id = character.Id,
        Name = character.Name,
        Stats = new ListCharactersForAdmin.Stats
        {
            Attributes = new()
            {
                Level = character.Stats.Attributes.Level,
                Strength = character.Stats.Attributes.Strength,
                Dexterity = character.Stats.Attributes.Dexterity,
                Intelligence = character.Stats.Attributes.Intelligence,
                Constitution = character.Stats.Attributes.Constitution,
                Spirit = character.Stats.Attributes.Spirit,
            },
            Resources = new()
            {
                Health = character.Stats.Resources.Health,
                Energy = character.Stats.Resources.Energy,
            },
        },
    };
}