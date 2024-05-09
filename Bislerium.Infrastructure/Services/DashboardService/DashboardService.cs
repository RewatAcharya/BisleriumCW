using Bislerium.Application.IServices;
using Bislerium.Domain.Enums;
using Bislerium.Domain.ViewModels;
using Bislerium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);


        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardVM> GetDashboardData(int year, int month)
        {
            DashboardVM dashboardVM = new DashboardVM()
            {
                TotalBlogPosts = GetTotalBlogPosts(),
                TotalUpvotes = GetTotalUpvotes(),
                TotalDownvotes = GetTotalDownvotes(),
                TotalComments = GetTotalComments(),

                MonthlyBlogPosts = GetMonthlyBlogPosts(year, month),
                MonthlyComments = GetMonthlyComments(year, month),
                MonthlyDownvotes = GetMonthlyDownvotes(year, month),
                MonthlyUpvotes = GetMonthlyUpvotes(year, month),
            };

            return dashboardVM;
        }

        public async Task<LineChart> GetDataRange()
        {
            throw new NotImplementedException();

        }

        public Monthly GetMonthlyBlogPosts(int year, int month)
        {
            var monthlyCounts = new Monthly();

            monthlyCounts.CurrentTotalBlogPosts = _context.Blogs.Count(x => x.CreatedAt.Year == year && x.CreatedAt.Month == month);

            int previousMonth = month - 1;
            int previousYear = year;
            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear--;
            }
            monthlyCounts.PreviousTotalBlogPosts = _context.Blogs.Count(x => x.CreatedAt.Year == previousYear && x.CreatedAt.Month == previousMonth);

            return monthlyCounts;
        }

        public Monthly GetMonthlyComments(int year, int month)
        {
            var monthlyCounts = new Monthly();

            monthlyCounts.CurrentTotalComments = _context.Comments.Count(x => x.CreatedAt.Year == year && x.CreatedAt.Month == month);

            int previousMonth = month - 1;
            int previousYear = year;
            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear--;
            }
            monthlyCounts.PreviousTotalComments = _context.Comments.Count(x => x.CreatedAt.Year == previousYear && x.CreatedAt.Month == previousMonth);

            return monthlyCounts;
        }

        public Monthly GetMonthlyDownvotes(int year, int month)
        {
            var monthlyCounts = new Monthly();

            monthlyCounts.CurrentTotalDownvotes = _context.LikeBlogs.Count(x => x.CreatedAt.Year == year && x.CreatedAt.Month == month && x.Reaction == Domain.Enums.ReactionType.DownVote);

            int previousMonth = month - 1;
            int previousYear = year;
            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear--;
            }
            monthlyCounts.PreviousTotalDownvotes = _context.LikeBlogs.Count(x => x.CreatedAt.Year == previousYear && x.CreatedAt.Month == previousMonth && x.Reaction == Domain.Enums.ReactionType.DownVote);

            return monthlyCounts;
        }

        public Monthly GetMonthlyUpvotes(int year, int month)
        {
            var monthlyCounts = new Monthly();

            monthlyCounts.CurrentTotalUpvotes = _context.LikeBlogs.Count(x => x.CreatedAt.Year == year && x.CreatedAt.Month == month && x.Reaction == Domain.Enums.ReactionType.UpVote);

            int previousMonth = month - 1;
            int previousYear = year;
            if (previousMonth == 0)
            {
                previousMonth = 12;
                previousYear--;
            }
            monthlyCounts.CurrentTotalUpvotes = _context.LikeBlogs.Count(x => x.CreatedAt.Year == previousYear && x.CreatedAt.Month == previousMonth && x.Reaction == Domain.Enums.ReactionType.UpVote);

            return monthlyCounts;
        }

        public async Task<List<LikeCountVM>> ChartLikeDislikes(int year, int month)
        {
            var likesByDay = await _context.LikeBlogs
               .Where(l => l.CreatedAt.Year == year && l.CreatedAt.Month == month)
               .GroupBy(l => l.CreatedAt.Day)
               .Select(g => new
               {
                   Day = g.Key,
                   Likes = g.Count(l => l.Reaction == ReactionType.UpVote),
                   Dislikes = g.Count(l => l.Reaction == ReactionType.DownVote)
               })
               .OrderBy(g => g.Day)
               .ToListAsync();

            var commentsByDay = await _context.Comments
                .Where(c => c.CreatedAt.Year == year && c.CreatedAt.Month == month)
                .GroupBy(c => c.CreatedAt.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    Comments = g.Count()
                })
                .OrderBy(g => g.Day)
                .ToListAsync();

            var blogsByDay = await _context.Blogs
                .Where(b => b.CreatedAt.Year == year && b.CreatedAt.Month == month)
                .GroupBy(b => b.CreatedAt.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    Blogs = g.Count()
                })
                .OrderBy(g => g.Day)
                .ToListAsync();

            var result = new List<LikeCountVM>();

            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                var likeData = likesByDay.FirstOrDefault(l => l.Day == day);
                var commentData = commentsByDay.FirstOrDefault(c => c.Day == day);
                var blogData = blogsByDay.FirstOrDefault(b => b.Day == day);

                if ((likeData?.Likes ?? 0) + (likeData?.Dislikes ?? 0) + (commentData?.Comments ?? 0) + (blogData?.Blogs ?? 0) > 0)
                {
                    result.Add(new LikeCountVM
                    {
                        Day = day,
                        Likes = likeData?.Likes ?? 0,
                        Dislikes = likeData?.Dislikes ?? 0,
                        Comment = commentData?.Comments ?? 0,
                        Blog = blogData?.Blogs ?? 0
                    });
                }
            }

            return result;
        }


        public int GetTotalBlogPosts()
        {
            return _context.Blogs.Count();
        }

        public int GetTotalComments()
        {
            return _context.Comments.Count();
        }

        public int GetTotalDownvotes()
        {
            return _context.LikeBlogs.Count(x => x.Reaction == Domain.Enums.ReactionType.DownVote);
        }

        public int GetTotalUpvotes()
        {
            return _context.LikeBlogs.Count(x => x.Reaction == Domain.Enums.ReactionType.UpVote);
        }
    }
}
