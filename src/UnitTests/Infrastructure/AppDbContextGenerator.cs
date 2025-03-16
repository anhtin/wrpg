using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Wrpg;

namespace Infrastructure;

public static class AppDbContextGenerator
{
    public static AppDbContext Create(IModel? model = null, object[]? entities = null) =>
        CreateInMemory(model, entities);

    public static AppDbContext CreateInMemory(IModel? model = null, object[]? entities = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        if (model is not null)
            optionsBuilder.UseModel(model);

        var options = optionsBuilder.Options;
        var dbContext = new AppDbContext(options);

        if (entities is not null)
        {
            dbContext.AddRange(entities);
            dbContext.SaveChanges();
        }

        return dbContext;
    }

    public static IModel CreateModel(Action<ModelBuilder> configure)
    {
        var builder = new ModelBuilder();
        configure(builder);
        return builder.FinalizeModel();
    }
}