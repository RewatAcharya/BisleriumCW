using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Users;
using Bislerium.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.Blogs
{
    public class LikeBlog : Base
    {
        public ReactionType Reaction { get; set; }

        [ForeignKey("Blog")]
        public Guid LikedBlog { get; set; }
        [ForeignKey("User")]
        public string LikedUser { get; set; }

        public virtual Blog? Blog { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
