using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IUpVoteBlogService
    {
        Task<Blog> UpdatePopulaityAsync(Guid blogId, ReactionType reaction, bool isCreated, bool isDelete);
        Task<LikeBlog> GetLike(Guid blogId, string userId);
        Task<bool> Vote(LikeBlog likeBlog); 
    }
}
