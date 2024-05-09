using Bislerium.Domain.Entity.BaseEntity;
using Bislerium.Domain.Entity.Blogs;
using Bislerium.Domain.Entity.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bislerium.Domain.Entity.Notifications
{
    public class PushNotification : Base
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        [ForeignKey("SendBy")]
        public string Sender { get; set; } 
        [ForeignKey("SendTo")]
        public string Receiver { get; set; }

        public virtual ApplicationUser? SendBy { get; set; }
        public virtual ApplicationUser? SendTo { get; set; }

    }
}
