using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using Bislerium.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IBlogService : IService<Blog>
    {
        Task<IEnumerable<Blog>> GetAllAsync(int pageNo, int sortBy);
        Task<IEnumerable<BlogHistory>> GetBlogDetailAsync(Guid id);
    }
}
