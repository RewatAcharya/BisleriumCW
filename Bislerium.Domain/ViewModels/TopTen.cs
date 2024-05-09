using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.ViewModels
{
    public class TopTen
    {
        public List<Blog> AllTimeBlog { get; set; }
        public List<ApplicationUser> AllTimeBlogger { get; set; }

        public List<Blog> MonthlyTopBlog { get; set; }
        public List<ApplicationUser> MonthlyTopBlogger { get; set; }

        public TopTen()
        {
            AllTimeBlog = new List<Blog>();
            AllTimeBlogger = new List<ApplicationUser>();
            MonthlyTopBlog = new List<Blog>();
            MonthlyTopBlogger = new List<ApplicationUser>();
        }
    }
}
