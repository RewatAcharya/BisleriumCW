using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface ICommentService : IService<Comment>
    {
        Task<IEnumerable<CommentHistory>> GetCommentDetailAsync(Guid id);
    }
}
