using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.ViewModels
{
    public class DashboardVM
    {
        public int TotalBlogPosts { get; set; }
        public int TotalUpvotes { get; set; }
        public int TotalDownvotes { get; set; }
        public int TotalComments { get; set; }

        public Monthly MonthlyBlogPosts { get; set; }
        public Monthly MonthlyUpvotes { get; set; }
        public Monthly MonthlyDownvotes { get; set; }
        public Monthly MonthlyComments { get; set; }
    }

    public class Monthly
    {
        public int CurrentTotalBlogPosts { get; set; }
        public int CurrentTotalUpvotes { get; set; }
        public int CurrentTotalDownvotes { get; set; }
        public int CurrentTotalComments { get; set; }

        public int PreviousTotalBlogPosts { get; set; }
        public int PreviousTotalUpvotes { get; set; }
        public int PreviousTotalDownvotes { get; set; }
        public int PreviousTotalComments { get; set; }
    }
}
