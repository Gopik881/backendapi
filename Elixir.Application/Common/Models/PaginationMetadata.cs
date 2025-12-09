namespace Elixir.Application.Common.Models;
public class PaginationMetadata
{
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int StartIndex => (CurrentPage - 1) * PageSize + 1;
    public int EndIndex => Math.Min((StartIndex-1) + PageSize, TotalItems);
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;

    public PaginationMetadata(int totalItems, int pageSize, int currentPage)
    {
        TotalItems = totalItems;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
    }
}
