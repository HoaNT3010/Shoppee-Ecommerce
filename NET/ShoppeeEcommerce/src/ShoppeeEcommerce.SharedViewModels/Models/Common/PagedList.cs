namespace ShoppeeEcommerce.SharedViewModels.Models.Common
{
    public class PagedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public int PageSize { get; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            Items = items;
        }
    }

    public static class PagedList
    {
        public static PagedList<T> Create<T>(List<T> items, int count, int pageNumber, int pageSize)
        {
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
