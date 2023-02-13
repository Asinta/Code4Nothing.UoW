namespace Code4Nothing.UoW.PagedCollections;

public interface IPagedCollection<T>
{
    int PageFrom { get; }
    int PageIndex { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
    IList<T> Items { get; }
}