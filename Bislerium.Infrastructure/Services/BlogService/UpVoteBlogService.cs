using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Notifications;
using Bislerium.Domain.Enums;
using Bislerium.Domain.Statics;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Services.HubService;
using Bislerium.Infrastructure.Services.NotificationServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Bislerium.Infrastructure.Services.BlogService
{
    public class UpVoteBlogService : IUpVoteBlogService
    {
        private readonly ApplicationDbContext _context;

        public UpVoteBlogService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Blog> UpdatePopulaityAsync(Guid blogId, ReactionType reaction, bool IsCreated, bool IsDelete)
        {
            BlogService blogService = new(_context);
            var blog = await blogService.GetByIdAsync(blogId);
            if (IsCreated)
            {
                if (reaction == ReactionType.UpVote)
                {
                    blog.UpVotes += 1;
                }
                else
                {
                    blog.DownVotes += 1;
                }
            }
            else
            {
                if (reaction == ReactionType.UpVote)
                {
                    blog.UpVotes -= 1;
                }
                else
                {
                    blog.DownVotes -= 1;
                }

            }
            blog.Popularity = SetStatics.CalculatePopularity(blog.UpVotes, blog.DownVotes, blog.Comments);
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<LikeBlog> GetLike(Guid blogId, string UserId)
        {
            return await _context.LikeBlogs.FirstOrDefaultAsync(x => x.LikedBlog == blogId && x.LikedUser == UserId);
        }

        public async Task<bool> Vote(LikeBlog likeBlog)
        {
            var blog = _context.Blogs.Include(x => x.User).FirstOrDefault(x => x.Id == likeBlog.LikedBlog);
            try
            {
                var existingLike = await GetLike(likeBlog.LikedBlog, likeBlog.LikedUser);
                if (existingLike != null)
                {
                    if ((existingLike.Reaction == ReactionType.UpVote && likeBlog.Reaction == ReactionType.DownVote)
                        || (existingLike.Reaction == ReactionType.DownVote && likeBlog.Reaction == ReactionType.UpVote))
                    {
                        var result = await new UpVoteBlogService(_context).UpdateAsync(existingLike.Id);

                        //if (result != null && likeBlog.LikedUser != likeBlog.Blog.Blogger)
                        //{
                        //    PushNotification notification = new()
                        //    {
                        //        Sender = likeBlog.LikedUser,
                        //        Receiver = likeBlog.Blog.Blogger,
                        //        Type = "Comment",
                        //        Message = $"Your {existingLike.Reaction} has been update to {likeBlog.Reaction} by " + likeBlog.User?.Name,
                        //    };

                        //    NotificationService service = new(_context);
                        //    await service.Create(notification);
                        //}

                        return true;
                    }
                    else
                    {
                        await new UpVoteBlogService(_context).DeleteVote(existingLike.Id);
                        var result = await new UpVoteBlogService(_context).UpdatePopulaityAsync(existingLike.LikedBlog, likeBlog.Reaction, false, true);

                        //if (result != null && likeBlog.LikedUser != likeBlog.Blog?.Blogger)
                        //{
                        //    PushNotification notification = new()
                        //    {
                        //        Sender = likeBlog.LikedUser,
                        //        Receiver = likeBlog.Blog.Blogger,
                        //        Type = "Comment",
                        //        Message = $"Your reaction has been deleted by " + likeBlog.User?.Name,
                        //    };

                        //    NotificationService service = new(_context);
                        //    await service.Create(notification);
                        //}

                        return false;
                    }
                }
                else
                {
                    _context.LikeBlogs.Add(likeBlog);
                    await _context.SaveChangesAsync();
                    var result = await new UpVoteBlogService(_context).UpdatePopulaityAsync(likeBlog.LikedBlog, likeBlog.Reaction, true, false);
                    //if (result != null && likeBlog.LikedUser != likeBlog.Blog.Blogger)
                    //{
                    //    PushNotification notification = new()
                    //    {
                    //        Sender = likeBlog.LikedUser,
                    //        Receiver = likeBlog.Blog.Blogger,
                    //        Type = "Comment",
                    //        Message = $"Your have received new reaction {likeBlog.Reaction} by " + likeBlog.User?.Name,
                    //    };

                    //    NotificationService service = new(_context);
                    //    await service.Create(notification);
                    //}
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while voting: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DeleteVote(Guid likeId)
        {
            try
            {
                var like = await _context.LikeBlogs.FirstOrDefaultAsync(x => x.Id == likeId);
                if (like == null)
                {
                    return false;
                }

                _context.LikeBlogs.Remove(like);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting vote: {ex.Message}");
                return false;
            }
        }

        private async Task<LikeBlog> UpdateAsync(Guid id)
        {
            var like = await _context.LikeBlogs.FirstOrDefaultAsync(x => x.Id == id);

            if (like != null)
            {
                BlogService blogService = new(_context);
                var blog = await blogService.GetByIdAsync(like.LikedBlog);


                if (like.Reaction == ReactionType.UpVote)
                {
                    like.Reaction = ReactionType.DownVote;

                    blog.UpVotes -= 1;
                    blog.DownVotes += 1;
                }
                else
                {
                    like.Reaction = ReactionType.UpVote;

                    blog.UpVotes += 1;
                    blog.DownVotes -= 1;
                }
                blog.Popularity = SetStatics.CalculatePopularity(blog.UpVotes, blog.DownVotes, blog.Comments);

                _context.LikeBlogs.Update(like);
                _context.Blogs.Update(blog);

                await _context.SaveChangesAsync();
            }
            return like;
        }
    }
}
