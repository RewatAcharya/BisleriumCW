using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Blogs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.History
{
    public class BlogHistory : Base
    {
        public required string Title { get; set; }

        public required string Content { get; set; }

        public string? ImageUrl { get; set; }

        [ForeignKey("Blog")]
        public Guid UpdatedBlog { get; set; }

        public virtual Blog? Blog { get; set; }
    }
}
