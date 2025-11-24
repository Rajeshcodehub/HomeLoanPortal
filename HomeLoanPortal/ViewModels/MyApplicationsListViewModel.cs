using System;
using System.Collections.Generic;

namespace HomeLoanPortal.ViewModels
{
    public class MyApplicationsListViewModel
    {
        public List<MyApplicationsViewModel> Items { get; set; } = new List<MyApplicationsViewModel>();

        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize == 0) return 1;
                return (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }
    }
}
