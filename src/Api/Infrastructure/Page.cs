namespace Wrpg;

public class Page<T>
{
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required int TotalPageCount { get; set; }
    public required int TotalItemCount { get; set; }
    public required IEnumerable<T> Items { get; set; }

    public static Page<T> From(int pageNumber, int pageSize, int totalItemCount, T[] items) => new()
    {
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPageCount = (int)Math.Ceiling((decimal)totalItemCount / pageSize),
        TotalItemCount = totalItemCount,
        Items = items,
    };

    public static Page<T> From(int pageNumber, int pageSize, int totalItemCount, IEnumerable<T> items) => new()
    {
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPageCount = (int)Math.Ceiling((decimal)totalItemCount / pageSize),
        TotalItemCount = totalItemCount,
        Items = items,
    };
}