using Microsoft.EntityFrameworkCore;

namespace Wrpg;

public static class QueryableExtensions
{
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var totalItemCount = await query.CountAsync();
        var items = await query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToArrayAsync();
        return Page<T>.From(pageNumber, pageSize, totalItemCount, items);
    }
}