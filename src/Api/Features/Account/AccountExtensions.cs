using Microsoft.EntityFrameworkCore;

namespace Wrpg;

public static class AccountExtensions
{
    public static TEndpointRouteBuilder MapAccountEndpoints<TEndpointRouteBuilder>(
        this TEndpointRouteBuilder builder)
        where TEndpointRouteBuilder : IEndpointRouteBuilder
    {
        return builder
            .MapCreateAccountEndpoint()
            .MapGetAccountEndpoint()
            .MapDeleteAccountEndpoint();
    }

    public static ModelBuilder AddAccountEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(Account.Configure);
        return modelBuilder;
    }
}