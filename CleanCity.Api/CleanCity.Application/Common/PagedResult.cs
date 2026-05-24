namespace CleanCity.Application.Common
{
    public sealed class PagedResult<T>
    {
        public List<T> Items { get; init; } = new();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling(TotalCount / (double)PageSize)
            : 0;
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}