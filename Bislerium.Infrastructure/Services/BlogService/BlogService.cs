using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.ViewModels;
using Bislerium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.BlogService
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Blog> CreateAsync(Blog entity)
        {
            await _context.Blogs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _context.Blogs.FindAsync(id);
            if (result != null)
            {
                _context.Blogs.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            var result = await _context.Blogs.Include(x => x.User).Include(x => x.BlogCategory).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<Blog>> GetAllAsync(int pageNumber, int no)
        {
            int pageSize = 5;

            int skip = (pageNumber - 1) * pageSize;

            var query = await _context.Blogs
                                .Include(x => x.User)
                                .Include(x => x.BlogCategory).ToListAsync();

            switch (no)
            {
                case 0:
                    query = query.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
                case 1:
                    query = query.OrderByDescending(x => x.Popularity).ToList();
                    break;
                default:
                    query = query.OrderBy(x => x.Id).ToList();
                    break;
            }

            var result =  query.Skip(skip).Take(pageSize).ToList();

            return (result);
        }


        public async Task<IEnumerable<BlogHistory>> GetBlogDetailAsync(Guid id)
        {
            var result = await _context.BlogHistories.Include(x => x.Blog).Where(x => x.UpdatedBlog == id).ToListAsync();
            return result;
        }

        public async Task<Blog> GetByIdAsync(Guid id)
        {
            return await _context.Blogs.Include(x => x.User).Include(x => x.BlogCategory).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Blog> UpdateAsync(Blog entity)
        {
            var originalBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.Id == entity.Id);

            if (originalBlog != null)
            {
                var originalHistory = new BlogHistory
                {
                    Title = originalBlog.Title,
                    Content = originalBlog.Content,
                    ImageUrl = originalBlog.ImageUrl,
                    UpdatedBlog = originalBlog.Id
                };

                _context.BlogHistories.Add(originalHistory);

                originalBlog.Title = entity.Title;
                originalBlog.Content = entity.Content;
                if (!string.IsNullOrEmpty(entity.ImageUrl))
                {
                    originalBlog.ImageUrl = entity.ImageUrl;
                }
                originalBlog.Blogger = entity.Blogger;
                originalBlog.Category = entity.Category;

                _context.Blogs.Update(originalBlog);

                await _context.SaveChangesAsync();
            }

            return entity;
        }
    }
}
