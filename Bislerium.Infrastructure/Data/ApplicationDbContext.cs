using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.Entity.Notifications;
using Bislerium.Domain.Entity.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-2TPLGS3\\SQLEXPRESS;Database=BisleriumCW;Trusted_Connection=True;TrustServerCertificate=True");
        }

        public DbSet<Category> BlogCategory { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikeBlog> LikeBlogs { get; set; }
        public DbSet<UpvoteComment> UpvoteComments { get; set; }
        public DbSet<BlogHistory> BlogHistories { get; set; }
        public DbSet<CommentHistory> CommentHistories { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
    }
}
