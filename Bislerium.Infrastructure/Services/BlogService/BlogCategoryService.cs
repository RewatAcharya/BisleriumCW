using Bislerium.Application.IServices;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Infrastructure.Services.BlogService
{
    public class BlogCategoryService : IBlogCategoryService
    {
        private readonly ApplicationDbContext _context;

        public BlogCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateAsync(Category entity)
        {
            await _context.BlogCategory.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _context.BlogCategory.FindAsync(id);
            if (result != null)
            {
                _context.BlogCategory.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            var result = await _context.BlogCategory.ToListAsync();
            return result;
        }

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _context.BlogCategory.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> UpdateAsync(Category entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
