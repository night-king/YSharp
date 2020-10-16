using System;
using System.Collections.Generic;
using System.Text;

namespace YSharp.Models
{
    public interface IPagedList<T>
    {
        int CurrentPage
        {
            get; set;
        }

        int PageSize
        {
            get; set;
        }

        int TotalItemCount
        {
            get; set;
        }

        IEnumerable<T> Results
        {
            get; set;
        }

        int PageCount
        {
            get;
        }
    }
    public class PagedList<T> : IPagedList<T>
    {
        public int CurrentPage
        {
            get; set;
        }

        public int PageSize
        {
            get; set;
        }

        public int TotalItemCount
        {
            get; set;
        }

        public IEnumerable<T> Results
        {
            get; set;
        }

        public int PageCount
        {
            get
            {
                if (TotalItemCount > 0 && PageSize > 0)
                    return (int)Math.Ceiling(TotalItemCount / (double)PageSize);
                return 0;
            }
        }
        public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int count)
        {
            Results = items;
            CurrentPage = pageIndex;
            PageSize = pageSize;
            TotalItemCount = count;
        }
    }
}

