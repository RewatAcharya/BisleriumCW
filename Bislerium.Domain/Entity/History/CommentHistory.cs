using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Blogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.History
{
    public class CommentHistory : Base
    {
        public required string Content { get; set; }

        [ForeignKey("Comment")]
        public Guid UpdatedComment { get; set; }

        public virtual Comment? Comment { get; set; }
    }
}
