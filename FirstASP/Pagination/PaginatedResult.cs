namespace FirstASP.Pagination;

public class PaginatedResult<T>(List<T> items, int totalCount, int currentPage, int pageSize)
{
    public List<T> Items { get; set; } = items;
    private int TotalCount { get; set; } = totalCount;
    public int CurrentPage { get; set; } = currentPage;
    private int PageSize { get; set; } = pageSize;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}