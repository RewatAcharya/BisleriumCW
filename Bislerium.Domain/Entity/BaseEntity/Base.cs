using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.BaseEntity
{
    public class Base
    {
        [Key]
        public Guid Id { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       // public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
