using Bislerium.Domain.Entity.Blogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Application.IServices
{
    public interface IUpVoteCommentService
    {
        Task<UpvoteComment> GetLike(Guid commentId, string userId);
        Task<bool> Vote(UpvoteComment comment);
        Task<bool> Delete(Guid id);
        Task<UpvoteComment> UpdateAsync(Guid id);
    }
}
