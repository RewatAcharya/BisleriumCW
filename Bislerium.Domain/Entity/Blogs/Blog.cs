using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.Blogs
{
    public class Blog : Base
    {
        [MinLength(1)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public int UpVotes { get; set; } = 0;
        public int DownVotes { get; set; } = 0;
        public int Comments { get; set; } = 0;

        public decimal Popularity { get; set; } = 0;

        [ForeignKey(nameof(BlogCategory))]
        public Guid Category { get; set; }
        
        [ForeignKey(nameof(User))]
        public string Blogger { get; set; }

        public string? ImageUrl { get; set; }
        //[NotMapped]
        //[DataType(DataType.Upload)]
        //public IFormFile? FileUpload { get; set; }

        public virtual Category? BlogCategory { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
