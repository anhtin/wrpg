namespace Wrpg;

public record DeleteEntity<TEntity>(TEntity Entity) where TEntity : class
{
    public void Execute(AppDbContext dbContext)
    {
        dbContext.Set<TEntity>().Remove(Entity);
    }
}