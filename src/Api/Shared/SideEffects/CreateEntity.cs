using Wrpg.Shared.Database;

namespace Wrpg.Shared.SideEffects;

public record CreateEntity<TEntity>(TEntity Entity) where TEntity : class
{
    public void Execute(AppDbContext dbContext)
    {
        dbContext.Set<TEntity>().Add(Entity);
    }
}