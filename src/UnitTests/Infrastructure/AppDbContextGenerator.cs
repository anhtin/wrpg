using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Wrpg.Shared.Database;

namespace Infrastructure;

public static class AppDbContextGenerator
{
    public static AppDbContext Create(IModel? model = null) => CreateInMemory(model);

    public static AppDbContext CreateInMemory(IModel? model = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        if (model is not null)
            optionsBuilder.UseModel(model);

        var options = optionsBuilder.Options;
        return new AppDbContext(options);
    }

    public static IModel CreateModel(Action<ModelBuilder> configure)
    {
        var builder = new ModelBuilder();
        configure(builder);
        return builder.FinalizeModel();
    }
}