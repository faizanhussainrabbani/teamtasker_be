namespace TeamTasker.Application.Common.Models
{
    /// <summary>
    /// Common parameters for pagination
    /// </summary>
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private int _pageNumber = 1;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }

        /// <summary>
        /// Sort by field
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction (asc or desc)
        /// </summary>
        public string? SortDirection { get; set; }

        /// <summary>
        /// Search query
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Get sort direction as boolean (true for ascending, false for descending)
        /// </summary>
        public bool SortAscending => string.IsNullOrEmpty(SortDirection) ? true : SortDirection.ToLower() == "asc";
    }
}
