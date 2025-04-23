using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TeamTasker.Application.Common.Models
{
    /// <summary>
    /// A paginated list of items
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// Items in the current page
        /// </summary>
        public List<T> Items { get; }

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        /// <summary>
        /// Creates a paginated list from a queryable source
        /// </summary>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);

            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Creates a paginated list from an in-memory list
        /// </summary>
        public static PaginatedList<T> Create(
            List<T> source,
            int pageNumber,
            int pageSize)
        {
            var count = source.Count;

            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
