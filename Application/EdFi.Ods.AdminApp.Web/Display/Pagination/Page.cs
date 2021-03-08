using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Web.Display.Pagination
{
    public class Page<T>
    {
        private readonly Func<int, int, Task<IEnumerable<T>>> GetApiRecords;

        public static readonly int DefaultPageSize = 20;

        public Page(Func<int, int, Task<IEnumerable<T>>> getApiRecords)
        {
            GetApiRecords = getApiRecords;
        }

        public static PagedList<T> Fetch(Func<int, int, Task<IEnumerable<T>>> getApiRecords, int pageNumber)
        {
            return Fetch(getApiRecords, pageNumber, DefaultPageSize);
        }

        public static PagedList<T> Fetch(Func<int, int, Task<IEnumerable<T>>> getApiRecords, int pageNumber, int pageSize)
        {
            var service = new Page<T>(getApiRecords);

            return service.PagedList(pageNumber, pageSize);
        }

        private PagedList<T> PagedList(int pageNumber, int pageSize)
        {
            if (pageSize >= 100)
            {
                throw new ArgumentException("Page numbers of 100 or greater are not currently supported due to limit constraints on the EdFi API");
            }

            var humanPageNumber = pageNumber == 0 ? 1 : pageNumber;

            var recordsToOffset = (humanPageNumber - 1) * pageSize;

            var records = GetApiRecords(recordsToOffset, pageSize + 1).Result.ToList();

            return new PagedList<T>
            {
                PageNumber = humanPageNumber,
                NextPageHasResults = records.Count > pageSize,
                Items = records.Take(pageSize)
            };
        }
    }

    public class PagedList<T>
    {
        public int PageNumber { get; set; }
        public bool NextPageHasResults { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
