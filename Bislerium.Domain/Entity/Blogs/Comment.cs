using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.Blogs
{
    public class Comment : Base
    {
        public string Content { get; set; } = null!;
        public int UpVotes { get; set; } = 0;
        public int DownVotes { get; set; } = 0;
        [ForeignKey("Blog")]
        public Guid CommentBlog { get; set; }
        [ForeignKey("User")]
        public string CommentUser { get; set; }

        public virtual Blog? Blog { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
