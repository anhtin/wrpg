using Microsoft.EntityFrameworkCore;
using Wrpg.Shared.SideEffects;

namespace Wrpg.UnitTests.Shared.SideEffects;

public class DeleteEntityTest
{
    [Fact]
    public void Tracks_entity_as_Deleted()
    {
        var entity = new Entity { Key = Guid.NewGuid() };
        var sideEffect = new DeleteEntity<Entity>(entity);

        var dbModel = AppDbContextGenerator.CreateModel(configure: builder =>
        {
            builder.Entity<Entity>()
                .HasKey(x => x.Key);
        });
        var dbContext = AppDbContextGenerator.Create(model: dbModel);
        sideEffect.Execute(dbContext);

        var entries = dbContext.ChangeTracker.Entries<Entity>();
        var entry = Assert.Single(entries);
        Assert.Equal(EntityState.Deleted, entry.State);
        Assert.Equivalent(entity, entry.Entity);
    }

    public class Entity
    {
        public required Guid Key { get; set; }
    }
}