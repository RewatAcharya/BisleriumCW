using Bislerium.Domain.Entity.BaseEntity;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.Domain.Entity.Blogs
{
    public class Category : Base
    {
        [Required]
        [Display(Name = "Category")]
        public string? NameOfCategory { get; set; }

        public string? Description { get; set; } 
    }
}