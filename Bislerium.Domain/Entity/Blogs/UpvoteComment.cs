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
    public class UpvoteComment : Base
    {
        public ReactionType Reaction { get; set; } 

        [ForeignKey("Comment")]
        public Guid LikedComment { get; set; }
        [ForeignKey("User")]
        public string LikedUser { get; set; }

        public virtual Comment? Comment { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
