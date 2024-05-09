using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.Entity.Notifications;
using Bislerium.Domain.Enums;
using Bislerium.Domain.Statics;
using Bislerium.Infrastructure.Data;
using Bislerium.Infrastructure.Services.NotificationServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.BlogService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment entity)
        {
            //Get the blog to update the popularity
            BlogService blogService = new(_context);
            var blog = await blogService.GetByIdAsync(entity.CommentBlog);

            blog.Comments += 1;
            blog.Popularity = SetStatics.CalculatePopularity(blog.UpVotes, blog.DownVotes, blog.Comments);

            await _context.Comments.AddAsync(entity);
            var result = _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            //if (result != null && entity.CommentUser != entity.Blog.Blogger)
            //{
            //    PushNotification notification = new()
            //    {
            //        Sender = entity.CommentUser,
            //        Receiver = entity.Blog.Blogger,
            //        Type = "Comment",
            //        Message = "You have received comment from "+entity.User.Name,
            //    };

            //    NotificationService service = new(_context);
            //    await service.Create(notification);
            //}

            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _context.Comments.FindAsync(id);
            if (result != null)
            {
                _context.Comments.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            var result = await _context.Comments.Include(x => x.Blog).Include(x => x.User).Include(x => x.Blog.BlogCategory).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<CommentHistory>> GetCommentDetailAsync(Guid id)
        {
            var result = await _context.CommentHistories.Include(x => x.Comment).Where(x => x.UpdatedComment == id).ToListAsync();
            return result;
        }

        public async Task<Comment> GetByIdAsync(Guid id)
        {
            return await _context.Comments.Include(x => x.Blog).Include(x => x.User).Include(x => x.Blog.BlogCategory).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment> UpdateAsync(Comment entity)
        {
            var originalComment = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(b => b.Id == entity.Id);

            if (originalComment != null)
            {
                var originalHistory = new CommentHistory
                {
                    Content = originalComment.Content,
                    UpdatedComment = originalComment.Id
                };

                _context.CommentHistories.Add(originalHistory);

                
                originalComment.Content = entity.Content;
                originalComment.CommentBlog = entity.CommentBlog;
                originalComment.CommentUser = entity.CommentUser;

                _context.Comments.Update(originalComment);

                await _context.SaveChangesAsync();
            }

            return entity;
        }
    }
}
