using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.Users
{
    [Table("FirebaseToken")]
    public class FirebaseUserToken
    {
        [Key]
        public Guid ID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public string? Token { get; set; }

    }

}
